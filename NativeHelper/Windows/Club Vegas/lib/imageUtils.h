#pragma once

concurrency::task<Platform::Array<byte>^> getThumbnailFromImage(Windows::Storage::StorageFile^ file, unsigned int maxSide);