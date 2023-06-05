#! /usr/bin/python

import sys
import os
import re
import boto3
import fnmatch
import mimetypes
import time

if len(sys.argv) < 4:
    print("Insufficient arguments: NATIVE_BUILD_TARGET COMMIT_HASH CANVAS_OUTPUT")
    exit(1)

NATIVE_BUILD_TARGET = sys.argv[1]
COMMIT_HASH = sys.argv[2]
CANVAS_OUTPUT = sys.argv[3]


def find(pattern, path):
    result = []
    for root, dirs, files in os.walk(path):
        for name in files:
            if fnmatch.fnmatch(name, pattern):
                result.append(os.path.join(root, name))
    return result


target_files = find("*", CANVAS_OUTPUT)

# ACCESS_KEY and SECRET_KEY must defined in env with specified keys
ACCESS_KEY = os.environ.get("AWS_ACCESS_KEY_ID")
SECRET_KEY = os.environ.get("AWS_SECRET_ACCESS_KEY")
BUCKET_NAME = "bagelgames"

if ACCESS_KEY is None:
    print("MISSING S3 ACCESS KEY")
    exit(1)
if SECRET_KEY is None:
    print("MISSING S3 SECRET KEY")
    exit(1)

s3 = boto3.client('s3', aws_access_key_id=ACCESS_KEY,
                  aws_secret_access_key=SECRET_KEY)

S3_CANVAS_FILE_PATH = os.path.join(
    "SLOTS1", "facebook_canvas", NATIVE_BUILD_TARGET, COMMIT_HASH, str(int(time.time() * 1000)))
for target_file in target_files:
    root_trimmed_target_file = target_file[target_file.index(os.sep) + 1:]
    s3_file_path = os.path.join(
        S3_CANVAS_FILE_PATH, root_trimmed_target_file).replace(os.sep, '/')
    print("%s -> %s" % (target_file, s3_file_path))
    extra_args = {"ACL": "public-read"}
    if re.match(r".*\.unityweb$", target_file):
        extra_args["ContentEncoding"] = "br"
    mimetype, _ = mimetypes.guess_type(target_file)
    if mimetype is not None:
        extra_args["ContentType"] = mimetype
    s3.upload_file(target_file, BUCKET_NAME,
                   s3_file_path, ExtraArgs=extra_args)
