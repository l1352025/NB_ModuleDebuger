﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace ElectricPowerDebuger.Common
{
    class Util
    {
        public static byte[] GetByteFromStringHex(string strSource, string strSeparate = "", bool bReverse = false)
        {
            byte tmp;
            int iLoop;
            byte[] bytes = null;

            try
            {
                if (strSeparate == "")
                {
                    strSource = strSource.Trim();
                }
                else
                {
                    strSource = strSource.Trim().Replace(strSeparate, "");
                }
                bytes = new byte[strSource.Length / 2];

                for (iLoop = 0; iLoop < bytes.Length; iLoop++)
                {
                    bytes[iLoop] = Convert.ToByte(strSource.Substring(iLoop * 2, 2), 16);
                }
                if (true == bReverse)
                {
                    for (iLoop = 0; iLoop < bytes.Length; iLoop++)
                    {
                        tmp = bytes[iLoop];
                        bytes[iLoop] = bytes[bytes.Length - 1 - iLoop];
                        bytes[bytes.Length - 1 - iLoop] = tmp;
                    }
                }
            }
            catch(Exception)
            {
                bytes = null;
            }

            return bytes;
        }
        public static string GetStringHexFromByte(byte[] DataByte, int iStart, int iLength, string strSeparate = "", bool Reverse = false)
        {
            string strResult = "";
            
            if(DataByte == null)
            {
                return strResult;
            }

            for (int iLoop = 0; iLoop < iLength; iLoop++)
            {
                if (Reverse == true)
                {
                    strResult += DataByte[iStart + iLength - 1 - iLoop].ToString("X2") + strSeparate;
                }
                else
                {
                    strResult += DataByte[iStart + iLoop].ToString("X2") + strSeparate;
                }
            }
            strResult.Trim();

            return strResult;
        }

        public static byte BcdToDec(byte bcd)
        {
            return (byte)(bcd - (bcd >> 4) * 6);
        }

        public static byte DecToBcd(byte dec)
        {
            return (byte)(dec + (dec / 10) * 6);
        }

        public static byte[] GetDlt645Frame(byte[] addr, byte[] data)
        {
            byte[] frame = new byte[10 + data.Length];
            byte index = 0, crc = 0, iLoop;

            frame[index++] = 0x68;
            addr.CopyTo(frame, index);
            index += (byte)addr.Length;
            frame[index++] = 0x68;

            data.CopyTo(frame, index);
            index += (byte)data.Length;

            for (iLoop = 0; iLoop < index; iLoop++)
            {
                crc += frame[iLoop];
            }
            frame[index++] = crc;
            frame[index++] = 0x16;

            return frame;
        }

#if false
        //将一个字节数组序列化为结构
        private IFormatter formatter = new BinaryFormatter();
        private ValueType deserializeByteArrayToInfoObj(byte[] bytes)
        {
            ValueType vt;
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            try
            {
                MemoryStream stream = new MemoryStream(bytes);
                stream.Position = 0;
                stream.Seek(0, SeekOrigin.Begin);
                vt = (ValueType)formatter.Deserialize(stream);
                stream.Close();
                return vt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //将一个结构序列化为字节数组
        private byte[] serializeInfoObjToByteArray(ValueType infoStruct)
        {
            if (infoStruct == null)
            {
                return null;
            }

            try
            {
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, infoStruct);

                byte[] bytes = new byte[(int)stream.Length];
                stream.Position = 0;
                int count = stream.Read(bytes, 0, (int)stream.Length);
                stream.Close();
                return bytes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// 将字节数组转换为结构体
        public object BytesToStruct(byte[] bytes, Type type)
        {
            //得到结构体大小
            int size = Marshal.SizeOf(type);
            Math.Log(size, 1);

            if (size > bytes.Length)
                return null;
            //分配结构大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将BYTE数组拷贝到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内容空间
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }
        /// 将结构转换为字节数组
        public byte[] StructTOBytes(object obj)
        {
            int size = Marshal.SizeOf(obj);
            //创建byte数组
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷贝到分配好的内存空间
            Marshal.StructureToPtr(obj, structPtr, false);
            //从内存空间拷贝到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }
#endif

    }
}
