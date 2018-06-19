using System;
using System.Runtime.InteropServices;
using Domain.BehaviourMessages;

namespace Server2
{
   /* [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 80)]
    public struct ObjectBox
    {
        [MarshalAs(UnmanagedType.R8)] private readonly double _posX;
        [MarshalAs(UnmanagedType.R8)] private readonly double _posY;
        [MarshalAs(UnmanagedType.R8)] private readonly double _posZ;
        [MarshalAs(UnmanagedType.R8)] private readonly double _quatX;
        [MarshalAs(UnmanagedType.R8)] private readonly double _quatY;
        [MarshalAs(UnmanagedType.R8)] private readonly double _quatZ;
        [MarshalAs(UnmanagedType.R8)] private readonly double _quatW;
        [MarshalAs(UnmanagedType.R8)] private readonly double _scaleX;
        [MarshalAs(UnmanagedType.R8)] private readonly double _scaleY;
        [MarshalAs(UnmanagedType.R8)] private readonly double _scaleZ;

        public ObjectBox(JediumTransformSnapshot GOB)
        {
            _posX = GOB.X;
            _posY = GOB.Y;
            _posZ = GOB.Z;
            _quatX = GOB.RotX;
            _quatY = GOB.RotY;
            _quatZ = GOB.RotZ;
            _quatW = GOB.RotW;
            _scaleX = GOB.ScaleX;
            _scaleY = GOB.ScaleY;
            _scaleZ = GOB.ScaleZ;
        }

        public static byte[] Serialize(ObjectBox data)
        {
            int rawSize = Marshal.SizeOf(typeof(ObjectBox));
            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(data, buffer, false);
            byte[] rawData = new byte[rawSize];
            Marshal.Copy(buffer, rawData, 0, rawSize);
            Marshal.FreeHGlobal(buffer); //Освобождает память, выделенную ранее из неуправляемой памяти процесса.
            return rawData;
        }

        public static ObjectBox Deserialize(byte[] rawData)
        {
            int rawsize = Marshal.SizeOf(typeof(ObjectBox));
            if (rawsize > rawData.Length)
                return default(ObjectBox);

            ObjectBox obj = default(ObjectBox);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);

            try
            {
                Marshal.Copy(rawData, 0, buffer, rawsize);
                obj = (ObjectBox) Marshal.PtrToStructure(buffer, typeof(ObjectBox));
            }
            catch (Exception ex)
            {
                var errstring = ex.ToString();
            }
            finally
            {
                Marshal.FreeHGlobal(buffer); //Освобождает память, выделенную ранее из неуправляемой памяти процесса.
            }

            //  Console.WriteLine("_______DESERIALIZED BOX:"+obj._posX+";"+obj._posY+";"+obj._posZ);

            return obj;
        }

        public JediumTransformSnapshot ToGameObjectBox(ObjectBox o)
        {
            //  return new JediumTransformSnaps(null, (float) o._posX, (float) o._posY, (float) o._posZ,
            //      (float) o._quatX, (float) o._quatW, (float) o._quatZ, (float) o._quatW,
            //      (float) o._scaleX, (float) o._scaleY, (float) o._scaleZ);
            return new JediumTransformSnapshot(Guid.Empty, (float) o._posX, (float) o._posY, (float) o._posZ,
                (float) o._quatX, (float) o._quatY, (float) o._quatZ, (float) o._quatW,
                (float) o._scaleX, (float) o._scaleY, (float) o._scaleZ);
            //{
            //    X = (float) o._posX,
            //    Y = (float) o._posY,
            //    Z = (float) o._posZ,
            //    RotX = (float) o._quatX,
            //    RotY = (float) o._quatX,
            //    RotZ = (float) o._quatX,
            //    RotW = (float) o._quatX,
            //    ScaleX = (float) o._scaleX,
            //    ScaleY = (float) o._scaleY,
            //    ScaleZ = (float) o._scaleZ
            //};
        }
    }*/
}