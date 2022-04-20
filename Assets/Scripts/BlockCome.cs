using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IronPython.Hosting;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Text;
using System;
using System.IO;
using System.Runtime.InteropServices;


public class BlockCome : MonoBehaviour
{
    public GameObject ArrowUp;
    public GameObject ArrowDown;
    public GameObject ArrowLeft;
    public GameObject ArrowRight;

    public GameObject BoxUp;
    public GameObject BoxDown;
    public GameObject BoxLeft;
    public GameObject BoxRight;

    Rigidbody2D rigidArrowUp;
    Rigidbody2D rigidArrowDown;
    Rigidbody2D rigidArrowLeft;
    Rigidbody2D rigidArrowRight;

    Rigidbody2D rigidBoxUp;
    Rigidbody2D rigidBoxDown;
    Rigidbody2D rigidBoxLeft;
    Rigidbody2D rigidBoxRight;

    int up, down, left, right;

    int SPEED = 100;
    int VISIBLE = 250;

    TcpClient client;
    string serverIP = "127.0.0.1";
    int port = 8000;
    byte[] receivedBuffer;
    StreamReader reader;
    bool socketReady = false;
    NetworkStream stream;

    // Start is called before the first frame update
    void Start()
    {
        rigidArrowUp = ArrowUp.GetComponent<Rigidbody2D>();
        rigidArrowDown = ArrowDown.GetComponent<Rigidbody2D>();
        rigidArrowLeft = ArrowLeft.GetComponent<Rigidbody2D>();
        rigidArrowRight = ArrowRight.GetComponent<Rigidbody2D>();

        rigidBoxUp = BoxUp.GetComponent<Rigidbody2D>();
        rigidBoxDown = BoxDown.GetComponent<Rigidbody2D>();
        rigidBoxLeft = BoxLeft.GetComponent<Rigidbody2D>();
        rigidBoxRight = BoxRight.GetComponent<Rigidbody2D>();

        rigidArrowUp.position = new Vector2(0, -8.5f);
        rigidArrowDown.position = new Vector2(0, 8.5f);
        rigidArrowLeft.position = new Vector2(8.5f, 0);
        rigidArrowRight.position = new Vector2(-8.5f, 0);

        rigidBoxUp.position = new Vector2(0, -8.5f);
        rigidBoxDown.position = new Vector2(0, 8.5f);
        rigidBoxLeft.position = new Vector2(8.5f, 0);
        rigidBoxRight.position = new Vector2(-8.5f, 0);

        up = down = left = right = 0;

        CheckReceive();
    }



    // Update is called once per frame
    void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                receivedBuffer = new byte[4];
                stream.Read(receivedBuffer, 0, receivedBuffer.Length);
                Array.Reverse(receivedBuffer);
                receivedBuffer = new byte[BitConverter.ToInt32(receivedBuffer, 0)];
                stream.Read(receivedBuffer, 0, receivedBuffer.Length);
                string msg = Encoding.UTF8.GetString(receivedBuffer);
                Debug.Log(msg);
                string[] data = msg.Split(' ');
                if (data[1] == "block")
                {
                    float pattern = -2.6f + (Int32.Parse(data[3]) - 1) * 1.3f;
                    if (data[2] == "up")
                    {
                        rigidArrowUp.position = new Vector2(pattern, -4.2f);
                        GameObject newBox = Instantiate(BoxUp);
                        Destroy(newBox, 10f);
                        newBox.GetComponent<Rigidbody2D>().position = new Vector2(pattern, -8.5f);
                        newBox.GetComponent<Rigidbody2D>().AddForce(Vector2.up * SPEED);
                        if (up == 0)
                            up = VISIBLE;
                    } else if (data[2] == "down")
                    {
                        rigidArrowDown.position = new Vector2(pattern, 4.2f);
                        GameObject newBox = Instantiate(BoxDown);
                        Destroy(newBox, 10f);
                        newBox.GetComponent<Rigidbody2D>().position = new Vector2(pattern, 8.5f);
                        newBox.GetComponent<Rigidbody2D>().AddForce(Vector2.down * SPEED);
                        if (down == 0)
                            down = VISIBLE;
                    } else if (data[2] == "left")
                    {
                        rigidArrowLeft.position = new Vector2(4.2f, pattern);
                        GameObject newBox = Instantiate(BoxLeft);
                        Destroy(newBox, 10f);
                        newBox.GetComponent<Rigidbody2D>().position = new Vector2(8.5f, pattern);
                        newBox.GetComponent<Rigidbody2D>().AddForce(Vector2.left * SPEED);
                        if (left == 0)
                            left = VISIBLE;
                    } else if (data[2] == "right")
                    {
                        rigidArrowRight.position = new Vector2(-4.2f, pattern);
                        GameObject newBox = Instantiate(BoxRight);
                        Destroy(newBox, 10f);
                        newBox.GetComponent<Rigidbody2D>().position = new Vector2(-8.5f, pattern);
                        newBox.GetComponent<Rigidbody2D>().AddForce(Vector2.right * SPEED);
                        if (right == 0)
                            right = VISIBLE;
                    }
                }
            }
        }
        if (up != 0)
        {
            up--;
            if (up == 0)
            {
                rigidArrowUp.position = new Vector2(0, -8.5f);
            }
        }
        if (down != 0)
        {
            down--;
            if (down == 0)
            {
                rigidArrowDown.position = new Vector2(0, 8.5f);
            }
        }
        if (left != 0)
        {
            left--;
            if (left == 0)
            {
                rigidArrowLeft.position = new Vector2(8.5f, 0);
            }
        }
        if (right != 0)
        {
            right--;
            if (right == 0)
            {
                rigidArrowRight.position = new Vector2(-8.5f, 0);
            }
        }

    }

    void CheckReceive()
    {
        if (socketReady) return;
        try
        {
            client = new TcpClient(serverIP, port);
            if (client.Connected)
            {
                stream = client.GetStream();
                Debug.Log("Connect Success");
                socketReady = true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    void OnApplicationQuit()
    {
        CloseSocket();
    }
    void CloseSocket()
    {
        if (!socketReady) return;
        reader.Close();
        client.Close();
        socketReady = false;
    }
}
