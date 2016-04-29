package ict.mxr.lightfield;

import java.awt.geom.AffineTransform;
import java.awt.image.AffineTransformOp;
import java.awt.image.BufferedImage;
import java.io.File;

import javax.imageio.ImageIO;

public class LightFieldStitcher {
	
	private static final int OUTPUT_ATLAS_WIDTH = 4096;

	/**
	 * @param args
	 */
	public static void main(String[] args) {

		try {
			if(args.length < 6) {
				
				System.out.println("Usage: <This JAR> <folder with frames> <image base name> <file extension> <number of images> <images per frame> <output format>");
				return;
			}
			
			int imagesPerFrame = Integer.parseInt(args[4]);
			int numberOfImages = Integer.parseInt(args[3]);
			int numberOfFrames = numberOfImages/imagesPerFrame;
			
			int numberOfAtlases = 10;

			for(int i = 0; i < numberOfFrames; i ++) {
				
				BufferedImage imgList[] = new BufferedImage[imagesPerFrame];
				int width = 0, height = 0, imagesPerRow = 0, imagesPerAtlas = 0;
				
				for(int j = 0; j < imagesPerFrame; j ++) {
						
					String path = args[0] + "\\" + args[1] + String.format("%0" + (int)Math.ceil(Math.log10(numberOfImages)) + "d", i*imagesPerFrame+(j+1)) + "." + args[2];
					
					BufferedImage src = ImageIO.read(new File(path));
					
					if(width == 0 && height == 0) {
						imagesPerAtlas = (int) Math.ceil( ((double)imagesPerFrame)/((double)numberOfAtlases) );
						imagesPerRow = (int) Math.ceil(Math.sqrt( imagesPerAtlas ));
						width = OUTPUT_ATLAS_WIDTH / imagesPerRow;
						height = (int) Math.ceil(((double) src.getHeight()*width)/((double)src.getWidth()));
					}
					
					AffineTransform at = new AffineTransform();
					at.scale(((float)width)/(float)src.getWidth(), ((float)height)/(float)src.getHeight());
					AffineTransformOp scaleOp = new AffineTransformOp(at, AffineTransformOp.TYPE_BILINEAR);
					imgList[j] = scaleOp.filter(src, null);
				}
				
				int atlasWidth = width * imagesPerRow;
				int atlasHeight = height * (int)Math.ceil(((double)imagesPerAtlas) / (double)imagesPerRow);
				
				int imgCounter = 0;
				
				for(int j = 0; j < numberOfAtlases; j ++) {
				
					BufferedImage atlas = new BufferedImage(atlasWidth, atlasHeight, BufferedImage.TYPE_INT_ARGB);

					for(int p = atlasHeight - height; p >= 0; p -= height) {
						for(int q = 0; q < atlasWidth; q += width) {
							
							for(int y = 0; y < height; y ++) {
								for(int x = 0; x < width; x ++) {
									
									atlas.setRGB(q + x, p + y, imgList[imgCounter%imagesPerFrame].getRGB(x, y));
								}
							}
							
							if(++imgCounter == imgList.length) break;
							if(imgCounter == (j+1)*imagesPerAtlas) break;
						}
						
						if(imgCounter == imgList.length) break;
						if(imgCounter == (j+1)*imagesPerAtlas) break;
					}
					
					ImageIO.write(atlas, args[5].toUpperCase(), new File(args[0] + "\\atlas"+i+"_"+j+"."+args[5].toLowerCase()));
				}
				
				imgList = null;
			}
			
		} catch (Exception e) {

			e.printStackTrace();
		}
	}
}
