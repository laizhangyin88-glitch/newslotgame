#! /usr/bin/python

import sys
import os
import re
import csv
from datetime import datetime
import boto3
import pytz
import fnmatch

if len(sys.argv) < 8:
  print("Insufficient arguments: TARGET_OS NATIVE_BUILD_TARGET COMMIT_HASH BUILD_VERSION UNITY_VERSION JOB_NAME ASSETBUNDLE")
  exit(1)

TARGET_OS = sys.argv[1]
NATIVE_BUILD_TARGET = sys.argv[2]
COMMIT_HASH = sys.argv[3]
BUILD_VERSION = sys.argv[4]
UNITY_VERSION = sys.argv[5]
JOB_NAME = sys.argv[6]
ASSETBUNDLE = sys.argv[7]

def getAssetbundleSizeRecord(name, size):
  return {'target_os': TARGET_OS, 'native_build_target': NATIVE_BUILD_TARGET, 'commit_hash': COMMIT_HASH, 'build_version': BUILD_VERSION, 'unity_version': UNITY_VERSION, 'job_name': JOB_NAME, 'assetbundle_name': name, 'size': size}

def find(pattern, path):
  result = []
  for root, dirs, files in os.walk(path):
    for name in files:
      if fnmatch.fnmatch(name, pattern):
        result.append(os.path.join(root, name))
  return result

assetbundles = find('*', ASSETBUNDLE)

assetbundle_size_records = {}
for assetbundle in assetbundles:
  assetbundle_size = os.path.getsize(assetbundle)
  assetbundle_name = re.sub(r'.manifest$', '', assetbundle)
  assetbundle_name = assetbundle_name.split(os.sep)[-1]
  # comment out below line to separate lang file size from game bundle size
  assetbundle_name = re.sub(r'lang$', '', assetbundle_name)
  if assetbundle_name in assetbundle_size_records:
    assetbundle_size_records[assetbundle_name]['size'] += assetbundle_size    
  else:
    assetbundle_size_records[assetbundle_name] = getAssetbundleSizeRecord(assetbundle_name, assetbundle_size)

# ACCESS_KEY and SECRET_KEY must defined in env with specified keys
ACCESS_KEY = os.environ.get('AWS_ACCESS_KEY_ID')
SECRET_KEY = os.environ.get('AWS_SECRET_ACCESS_KEY')

BUCKET_NAME = 'bagelcode-slots1-fixtures'
DATE_FORMAT = '%Y-%m-%d'
pst_date = datetime.now(tz=pytz.timezone('US/Pacific'))

result_file_name = 'assetbundle_size.csv'
S3_FILE_NAME = 'assetbundle_size/dt=%s/%s' % (pst_date.strftime(DATE_FORMAT), result_file_name)
print(S3_FILE_NAME)

if ACCESS_KEY is None:
  print("MISSING S3 ACCESS KEY")
  exit(1)
if SECRET_KEY is None:
  print("MISSING S3 SECRET KEY")
  exit(1)

s3 = boto3.client('s3', aws_access_key_id = ACCESS_KEY, aws_secret_access_key = SECRET_KEY)

download_success = True
try:
  download_file_name = 'stored_' + result_file_name
  s3.download_file(BUCKET_NAME, S3_FILE_NAME, download_file_name)
  print("size track data found for today. append to that file")
  result_file_name = download_file_name
except Exception as e:
  print(e)
  if '404' in str(e):
    print("size track data not found for today. create new one")
  download_success = False

csvfile = open(result_file_name, 'a' if download_success else 'w')
FIELD_NAMES = ['target_os', 'native_build_target', 'commit_hash', 'build_version', 'unity_version', 'job_name', 'assetbundle_name', 'size']
writer = csv.DictWriter(csvfile, fieldnames=FIELD_NAMES)
if not download_success:
  writer.writeheader()
for assetbundle_name in assetbundle_size_records:
  writer.writerow(assetbundle_size_records[assetbundle_name])
csvfile.close()

s3.upload_file(result_file_name, BUCKET_NAME, S3_FILE_NAME)
