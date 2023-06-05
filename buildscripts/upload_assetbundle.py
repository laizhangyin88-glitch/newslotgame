import requests
import sys
import os
import fnmatch
from requests_toolbelt import MultipartEncoder

if len(sys.argv) < 6:
    print("Insufficient arguments: ASSETS_DIRECTORY UPLOAD_URL X_BYPASS_KEY COMMENT ASSET_VERSION IS_FOR_TEST(optional)")
    exit(1)

ASSETS_DIRECTORY = sys.argv[1]
UPLOAD_URL = sys.argv[2]
X_BYPASS_KEY = sys.argv[3]
COMMENT = sys.argv[4]
ASSET_VERSION = sys.argv[5]
IS_FOR_TEST = sys.argv[6] if len(sys.argv) >= 7 else "false"


def find(pattern, path):
    result = []
    for root, dirs, files in os.walk(path):
        for name in files:
            if fnmatch.fnmatch(name, pattern):
                result.append(os.path.join(root, name))
    return result


assetbundles = find('*', ASSETS_DIRECTORY)
if len(assetbundles) == 0:
    print("Assetbundle not found")
    exit(1)

files = [
    ('comment', (None, COMMENT)),
    ('dlc_asset_list', (None, '[]')),
    ('is_for_test', (None, IS_FOR_TEST)),
    ('asset_version', (None, ASSET_VERSION))
]

open_file_list = []
for assetbundle in assetbundles:
    print(assetbundle)
    open_file = open(assetbundle, 'rb')
    files.append(('file', (assetbundle, open_file)))
    open_file_list.append(open_file)

encoder = MultipartEncoder(fields=files)
response = requests.post(
    UPLOAD_URL,
    headers={
        'Content-Type': encoder.content_type,
        'X-bypass-key': X_BYPASS_KEY
    },
    data=encoder
)

for open_file in open_file_list:
    open_file.close()

if response.status_code == 404:
    print("Successful asset bundle upload")
else:
    # successful upload redirects to root(/) of the domain which respond with 404
    print("Response status code is not 404, but %d" % response.status_code)
    exit(1)
