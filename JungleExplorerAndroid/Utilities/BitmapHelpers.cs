using System;
using System.IO;

using Android.Graphics;
using System.Threading.Tasks;
using Java.IO;


namespace JungleExplorer
{
	public static class BitmapHelpers
	{
		/// <summary>
		/// Load the image from the device, and resize it to the specified dimensions.
		/// </summary>
		/// <returns>The and resize bitmap.</returns>
		/// <param name="fileName">File name.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public static Bitmap LoadAndResizeBitmap(string fileName, BitmapFactory.Options options, int width, int height)
		{
			// First we get the the dimensions of the file on disk

			BitmapFactory.DecodeFile(fileName, options);

			// Next we calculate the ratio that we need to resize the image by
			// in order to fit the requested dimensions.
			int outHeight = options.OutHeight;
			int outWidth = options.OutWidth;
			int inSampleSize = 1;
			if(height != 0 && width != 0)
			if (outHeight > height || outWidth > width)
			{
				inSampleSize = outWidth > outHeight
					? outHeight / height
					: outWidth / width;
			}

			// Now we will load the image and have BitmapFactory resize it for us.
			options.InSampleSize = inSampleSize;
			options.InJustDecodeBounds = false;
			Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

			return resizedBitmap;
		}
			
		public async static Task<BitmapFactory.Options> GetBitmapOptionsOfImageAsync(string path)
		{
			BitmapFactory.Options options = new BitmapFactory.Options
			{
				InJustDecodeBounds = true
			};

			// The result will be null because InJustDecodeBounds == true.
			var result=  BitmapFactory.DecodeFileAsync(path, options);

			int imageHeight = options.OutHeight;
			int imageWidth = options.OutWidth;


			return options;
		}

		public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			// Raw height and width of image
			float height = options.OutHeight;
			float width = options.OutWidth;
			double inSampleSize = 1D;

			if (height > reqHeight || width > reqWidth)
			{
				int halfHeight = (int)(height / 2);
				int halfWidth = (int)(width / 2);

				// Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
				while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
				{
					inSampleSize *= 2;
				}
			}

			return (int)inSampleSize;
		}

		public async static Task<Bitmap> LoadScaledDownBitmapForDisplayAsync(string path, BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			// Calculate inSampleSize
			options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;

			return await BitmapFactory.DecodeFileAsync(path, options);
		}
	}
}

