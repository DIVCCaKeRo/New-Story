using UnityEngine;
using System.Collections;

//using OpenCVForUnity;
using System;
using System.Collections.Generic;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;
using Vuforia;

public class PostRenderToMatSample : MonoBehaviour
{

    public GameObject quad;
    public GameObject material;
    public UnityEngine.GameObject backgroundplane;
    public Camera mainCamera;
    Mat cameraMat;
    Texture2D cameraTexture;
    Texture2D outputTexture;
    Color32[] colors;
    Mat mSepiaKernel;
    private Size mSize0;
    private Mat mIntermediateMat;
    bool check = false;

    public enum modeType
    {
        First_Check,
        Second_Check,
        Third_Check,
        Draw_Mode,
    }
    public modeType mode;


    // Use this for initialization
    void Start()
    {

        cameraMat = new Mat(Screen.height, Screen.width, MatType.CV_8UC3);

        Debug.Log("imgMat dst ToString " + cameraMat.ToString());


        cameraTexture = new Texture2D(cameraMat.Cols, cameraMat.Rows, TextureFormat.ARGB32, false);
        outputTexture = new Texture2D(cameraMat.Cols, cameraMat.Rows, TextureFormat.ARGB32, false);


        colors = new Color32[outputTexture.width * outputTexture.height];


        mainCamera.orthographicSize = cameraTexture.height;
        quad.transform.localScale = new Vector3(cameraTexture.width, cameraTexture.height, quad.transform.localScale.z);
        quad.GetComponent<Renderer>().material.mainTexture = outputTexture;



        //sepia
        //mSepiaKernel = new Mat (4, 4, CvType.CV_32F);
        //mSepiaKernel.put (0, 0, /* R */0.189f, 0.769f, 0.393f, 0f);
        //mSepiaKernel.put (1, 0, /* G */0.168f, 0.686f, 0.349f, 0f);
        //mSepiaKernel.put (2, 0, /* B */0.131f, 0.534f, 0.272f, 0f);
        //mSepiaKernel.put (3, 0, /* A */0.000f, 0.000f, 0.000f, 1f);


        // pixelize
        mIntermediateMat = new Mat();
        mSize0 = new Size();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPostRender()
    {

        //int sleepTime = 1000 / 25;
        Mat image = new Mat(); // Frame image buffer  
        //Mat test = new Mat();
        Mat thr = new Mat(), frame = new Mat(), dst = new Mat(), hsvImg = new Mat(), filter = new Mat();

        //Utils.texture2DToMat (cameraTexture, cameraMat);
        //image = cameraMat;

        //Renderer renderer = new Renderer();
        //renderer = backgroundplane.GetComponent<Renderer>();
        Texture tex = backgroundplane.GetComponent<Renderer>().material.mainTexture;

        if (mode == modeType.First_Check)//Green
        {


            cameraTexture = (Texture2D)tex;

            //co = cameraTexture.GetPixel()
            image = Mat.FromImageData(cameraTexture.EncodeToJPG());
            //Mat image = new Mat(cameraTexture.height, cameraTexture.width, MatType.CV_8UC3, cameraTexture.GetRawTextureData());
            //Cv2.CvtColor(image, image, ColorConversion.BgrToRgb);
            //Cv2.Flip(image, image, FlipMode.XY);

            //Imgproc.putText (cameraMat, "ORIGINAL MODE " + cameraTexture.width + "x" + cameraTexture.height, new Point (5, cameraTexture.height - 5), Core.FONT_HERSHEY_PLAIN, 1.0, new Scalar (255, 0, 0, 255));
            Cv2.GaussianBlur(image, image, Cv.Size(3, 3), 0, 0);
            Cv2.CvtColor(image, hsvImg, ColorConversion.BgrToHsv);
            Cv2.InRange(hsvImg, new Scalar(60, 144, 39), new Scalar(70, 255, 144), filter);
            Cv2.Threshold(filter, thr, 25, 255, ThresholdType.Binary); // Threshold the gray
            Cv2.Erode(thr, thr, new Mat(), new Point(-1, -1), 3);
            Cv2.Dilate(thr, thr, new Mat(), new Point(-1, -1), 3);

            dst = image;
            //Cv2.ImShow("imgae", image);

            Point[][] contours; // Vector for storing contour
            List<List<Point>> contours_poly = new List<List<Point>>();
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(thr, out contours, out hierarchy, ContourRetrieval.Tree, ContourChain.ApproxSimple, new Point(0, 0)); // Find the contours in the image
            for (int j = 0; j < contours.Length; j++)
                contours_poly.Add(new List<Point>());
            int i = 0;
            for (i = 0; i < contours.Length; i++)
            {
                Cv2.ApproxPolyDP(InputArray.Create(contours[i]), OutputArray.Create(contours_poly[i]), 3, true);
                Point[] poly = Cv2.ApproxPolyDP(contours[i], 3, true);
                if (poly.Length == 4)
                {
                    image.CopyTo(dst);
                    Cv2.Line(dst, poly[0], poly[1], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                    Cv2.Line(dst, poly[1], poly[2], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                    Cv2.Line(dst, poly[2], poly[3], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                    Cv2.Line(dst, poly[3], poly[0], new Scalar(0, 0, 255), 3, LineType.Link8, 0);


                    material.SetActive(true);
                    //material.transform.localPosition = Vector3.Lerp(material.transform.localPosition, new Vector3((poly[0].X+poly[2].X)/2, (poly[0].Y + poly[2].Y) / 2, 0.16f), 50);
                    check = true;
                    break;
                }
            }

            Cv2.ImShow("dst", dst);
            if (check == true)
                mode = modeType.Draw_Mode;
        }
        else if (mode == modeType.Second_Check)//Brown
        {

            cameraTexture = (Texture2D)tex;

            //co = cameraTexture.GetPixel()
            image = Mat.FromImageData(cameraTexture.EncodeToJPG());
            //Mat image = new Mat(cameraTexture.height, cameraTexture.width, MatType.CV_8UC3, cameraTexture.GetRawTextureData());
            //Cv2.CvtColor(image, image, ColorConversion.BgrToRgb);
            //Cv2.Flip(image, image, FlipMode.XY);

            //Imgproc.putText (cameraMat, "ORIGINAL MODE " + cameraTexture.width + "x" + cameraTexture.height, new Point (5, cameraTexture.height - 5), Core.FONT_HERSHEY_PLAIN, 1.0, new Scalar (255, 0, 0, 255));
            Cv2.GaussianBlur(image, image, Cv.Size(3, 3), 0, 0);
            Cv2.CvtColor(image, hsvImg, ColorConversion.BgrToHsv);
            Cv2.InRange(hsvImg, new Scalar(0, 102, 25), new Scalar(179, 255, 89), filter);
            Cv2.Threshold(filter, thr, 25, 255, ThresholdType.Binary); // Threshold the gray
            Cv2.Erode(thr, thr, new Mat(), new Point(-1, -1), 3);
            Cv2.Dilate(thr, thr, new Mat(), new Point(-1, -1), 3);

            dst = image;
            //Cv2.ImShow("imgae", image);

            Point[][] contours; // Vector for storing contour
            List<List<Point>> contours_poly = new List<List<Point>>();
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(thr, out contours, out hierarchy, ContourRetrieval.Tree, ContourChain.ApproxSimple, new Point(0, 0)); // Find the contours in the image
            for (int j = 0; j < contours.Length; j++)
                contours_poly.Add(new List<Point>());
            int i = 0;
            for (i = 0; i < contours.Length; i++)
            {
                Cv2.ApproxPolyDP(InputArray.Create(contours[i]), OutputArray.Create(contours_poly[i]), 3, true);
                Point[] poly = Cv2.ApproxPolyDP(contours[i], 3, true);
                if (poly.Length == 4)
                {
                    image.CopyTo(dst);
                    Cv2.Line(dst, poly[0], poly[1], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                    Cv2.Line(dst, poly[1], poly[2], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                    Cv2.Line(dst, poly[2], poly[3], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                    Cv2.Line(dst, poly[3], poly[0], new Scalar(0, 0, 255), 3, LineType.Link8, 0);


                    material.SetActive(true);
                    //material.transform.localPosition = Vector3.Lerp(material.transform.localPosition, new Vector3((poly[0].X+poly[2].X)/2, (poly[0].Y + poly[2].Y) / 2, 0.16f), 50);
                    check = true;
                    break;
                }
            }

            Cv2.ImShow("dst", dst);
            if (check == true)
                mode = modeType.Draw_Mode;

        }
        else if (mode == modeType.Third_Check)//Red
        {
            cameraTexture = (Texture2D)tex;

            //co = cameraTexture.GetPixel()
            image = Mat.FromImageData(cameraTexture.EncodeToJPG());
            //Mat image = new Mat(cameraTexture.height, cameraTexture.width, MatType.CV_8UC3, cameraTexture.GetRawTextureData());
            //Cv2.CvtColor(image, image, ColorConversion.BgrToRgb);
            //Cv2.Flip(image, image, FlipMode.XY);

            //Imgproc.putText (cameraMat, "ORIGINAL MODE " + cameraTexture.width + "x" + cameraTexture.height, new Point (5, cameraTexture.height - 5), Core.FONT_HERSHEY_PLAIN, 1.0, new Scalar (255, 0, 0, 255));
            Cv2.GaussianBlur(image, image, Cv.Size(3, 3), 0, 0);
            Cv2.CvtColor(image, hsvImg, ColorConversion.BgrToHsv);
            Cv2.InRange(hsvImg, new Scalar(0, 193, 84), new Scalar(179, 255, 146), filter);
            Cv2.Threshold(filter, thr, 25, 255, ThresholdType.Binary); // Threshold the gray
            Cv2.Erode(thr, thr, new Mat(), new Point(-1, -1), 3);
            Cv2.Dilate(thr, thr, new Mat(), new Point(-1, -1), 3);

            dst = image;
            //Cv2.ImShow("imgae", image);

            Point[][] contours; // Vector for storing contour
            List<List<Point>> contours_poly = new List<List<Point>>();
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(thr, out contours, out hierarchy, ContourRetrieval.Tree, ContourChain.ApproxSimple, new Point(0, 0)); // Find the contours in the image
            for (int j = 0; j < contours.Length; j++)
                contours_poly.Add(new List<Point>());
            int i = 0;
            for (i = 0; i < contours.Length; i++)
            {
                Cv2.ApproxPolyDP(InputArray.Create(contours[i]), OutputArray.Create(contours_poly[i]), 3, true);
                Point[] poly = Cv2.ApproxPolyDP(contours[i], 3, true);
                if (poly.Length == 4)
                {
                    image.CopyTo(dst);
                    Cv2.Line(dst, poly[0], poly[1], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                    Cv2.Line(dst, poly[1], poly[2], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                    Cv2.Line(dst, poly[2], poly[3], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                    Cv2.Line(dst, poly[3], poly[0], new Scalar(0, 0, 255), 3, LineType.Link8, 0);


                    material.SetActive(true);
                    //material.transform.localPosition = Vector3.Lerp(material.transform.localPosition, new Vector3((poly[0].X+poly[2].X)/2, (poly[0].Y + poly[2].Y) / 2, 0.16f), 50);
                    check = true;
                    break;
                }
            }

            Cv2.ImShow("dst", dst);
            if (check == true)
                mode = modeType.Draw_Mode;

        }
        else if (mode == modeType.Draw_Mode)
        {
            //Imgproc.resize (cameraMat, mIntermediateMat, mSize0, 0.2, 0.2, Imgproc.INTER_NEAREST);
            //Imgproc.resize (mIntermediateMat, cameraMat, cameraMat.size (), 0.0, 0.0, Imgproc.INTER_NEAREST);

            //Imgproc.putText (cameraMat, "PIXELIZE MODE" + cameraTexture.width + "x" + cameraTexture.height, new Point (5, cameraTexture.height - 5), Core.FONT_HERSHEY_PLAIN, 1.0, new Scalar (255, 0, 0, 255));

        }

        //Utils.matToTexture2D (cameraMat, outputTexture, colors);
    }

    void OnGUI()
    {
        float screenScale = Screen.width / 500.0f;
        Matrix4x4 scaledMatrix = Matrix4x4.Scale(new Vector3(screenScale, screenScale, screenScale));
        GUI.matrix = scaledMatrix;


        GUILayout.BeginVertical();

        if (GUILayout.Button("First Check"))
        {
            mode = modeType.First_Check;
        }

        if (GUILayout.Button("Second Check"))
        {
            mode = modeType.Second_Check;
        }

        if (GUILayout.Button("Third Check"))
        {
            mode = modeType.Third_Check;
        }

        if (GUILayout.Button("Draw Mode"))
        {
            mode = modeType.Draw_Mode;
        }
        GUILayout.EndVertical();
    }
}