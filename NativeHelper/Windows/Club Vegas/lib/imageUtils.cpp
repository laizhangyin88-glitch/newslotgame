#include "pch.h"
#include "imageUtils.h"

using namespace concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Graphics::Imaging;

task<BitmapDecoder^> readImageFile(StorageFile^ file) {
	return create_task(file->OpenAsync(FileAccessMode::Read)).then([](IRandomAccessStream^ stream) {
		return BitmapDecoder::CreateAsync(stream);
	});
}

task<Platform::Array<byte>^> readBytesFromStream(Streams::IInputStream^ stream, unsigned int size) {
	auto reader = ref new Windows::Storage::Streams::DataReader(stream);
	return create_task(reader->LoadAsync(size)).then([reader, stream](unsigned int size) {
		Platform::Array<byte>^ buf = ref new Platform::Array<byte>(size);
		reader->ReadBytes(buf);
		delete reader;
		return buf;
	});
}

task<Platform::Array<byte>^> convertToJPEG(BitmapDecoder^ imageDecoder) {
	auto inMemoryStream = ref new InMemoryRandomAccessStream();

	// Prepare set of properties for the bitmap
	BitmapPropertySet^ propertySet = ref new BitmapPropertySet();
	// Set ImageQuality
	BitmapTypedValue^ qualityValue = ref new BitmapTypedValue(1, PropertyType::Single);
	propertySet->Insert("ImageQuality", qualityValue);

	return create_task(BitmapEncoder::CreateAsync(BitmapEncoder::JpegEncoderId, inMemoryStream, propertySet))
		.then([inMemoryStream, imageDecoder](BitmapEncoder^ imageEncoder) {
			return create_task(imageDecoder->GetPixelDataAsync())
				.then([imageEncoder, imageDecoder](PixelDataProvider^ pixel) {
				imageEncoder->SetPixelData(imageDecoder->BitmapPixelFormat, imageDecoder->BitmapAlphaMode,
					imageDecoder->PixelWidth, imageDecoder->PixelHeight,
					imageDecoder->DpiX, imageDecoder->DpiY,
					pixel->DetachPixelData());
				return imageEncoder->FlushAsync();
			}).then([inMemoryStream]() {
				unsigned int size = (unsigned int)inMemoryStream->Size;
				return readBytesFromStream(inMemoryStream->GetInputStreamAt(0), size);
			});
		});
}

task<InMemoryRandomAccessStream^> resize(BitmapDecoder^ imageDecoder, unsigned int maxSide) {
	auto resizedStream = ref new InMemoryRandomAccessStream();
	return create_task(BitmapEncoder::CreateForTranscodingAsync(resizedStream, imageDecoder))
		.then([imageDecoder, maxSide](BitmapEncoder^ encoder) {
		auto originalPixelWidth = imageDecoder->PixelWidth;
		auto originalPixelHeight = imageDecoder->PixelHeight;
		double widthRatio = (double)maxSide / originalPixelWidth;
		double heightRatio = (double)maxSide / originalPixelHeight;
		unsigned int aspectHeight = maxSide;
		unsigned int aspectWidth = maxSide;
		unsigned int cropX = 0, cropY = 0;
		unsigned int scaledSize = (unsigned int)maxSide;
		if (originalPixelWidth > originalPixelHeight)
		{
			aspectWidth = (unsigned int)(heightRatio * originalPixelWidth);
			cropX = (aspectWidth - aspectHeight) / 2;
		}
		else
		{
			aspectHeight = (unsigned int)(widthRatio * originalPixelHeight);
			cropY = (aspectHeight - aspectWidth) / 2;
		}
		//you can adjust interpolation and other options here, so far linear is fine for thumbnails
		encoder->BitmapTransform->InterpolationMode = BitmapInterpolationMode::Cubic;
		encoder->BitmapTransform->ScaledHeight = aspectHeight;
		encoder->BitmapTransform->ScaledWidth = aspectWidth;
		auto bounds = BitmapBounds();
		bounds.Width = scaledSize;
		bounds.Height = scaledSize;
		bounds.X = cropX;
		bounds.Y = cropY;
		encoder->BitmapTransform->Bounds = bounds;
		return encoder->FlushAsync();
	}).then([resizedStream]() {
		return resizedStream;
	});
}

task<Platform::Array<byte>^> getThumbnailFromImage(StorageFile^ file, unsigned int maxSide) {
	return readImageFile(file).then([maxSide](BitmapDecoder^ decoder) {
		return resize(decoder, maxSide)
			.then([](InMemoryRandomAccessStream^ memoryStream) {
			memoryStream->Seek(0);
			return BitmapDecoder::CreateAsync(memoryStream);
		});
	}).then([](BitmapDecoder^ newDecoder) {
		return convertToJPEG(newDecoder);
	});
}
