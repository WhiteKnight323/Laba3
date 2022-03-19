# Welcome here!
  The purpose of the work:

  Develop a program that simulates the k-means clustering algorithm. The initial data is an image containing objects of different colors on a plain background. The following procedures must be implemented in the program with the output of the result on the screen:
  1) Image upload;
  2) Setting manually the number of clusters k
  3) Implementation of the k-means algorithm, where R, G, B pixel values, or brightness values ​​are used as features; the result is a mask image containing k colors corresponding to the formed clusters.

  Results of the work:

  When the program starts, the window shown in Fig. 1. This window contains buttons for selecting an image, the number of clusters (2-64) and the algorithm implementation method (by color or by brightness)

![image](https://user-images.githubusercontent.com/93475400/159118255-4def4a6b-1af6-4e8a-8f8c-ea0e1cc5d610.png)
Fig. 1 Startup window

  After selecting the image, it is transformed in accordance with the selected parameters: Fig. 2 by color, Fig. 3 for brightness.

![image](https://user-images.githubusercontent.com/93475400/159118273-e90d9afd-d332-4adc-adca-dd17ba4a50fe.png)
Fig. 2 Image clustering by 2 color levels

![image](https://user-images.githubusercontent.com/93475400/159118294-07082aa1-27a3-460b-8609-10112c9bc219.png)
Fig. 3 Image clustering by 2 brightness levels

  In the case of clustering by brightness, the following happens: the image is converted from the RGB color model to HSV, and then clustering occurs only in 3 components - brightness.
