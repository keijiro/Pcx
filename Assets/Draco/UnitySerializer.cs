using UnityEngine;
using System.Collections.Generic;
using System;

// Attribution
// https://forum.unity.com/threads/converting-float-to-byte.85128/

public class UnitySerializer : MonoBehaviour{

	private List<byte> byteStream = new List<byte>();
	private byte[] byteArray;
	private int index = 0;

	/// <summary>
	/// Returns the stream as a Byte Array
	/// </summary>
	public byte[] ByteArray  
	{
		get
		{
			if ( byteArray == null || byteStream.Count != byteArray.Length)
				byteArray = byteStream.ToArray();

			return byteArray;
		}
	}

	/// <summary>
	/// Create a new empty stream
	/// </summary>
	public UnitySerializer()
	{

	}

	/// <summary>
	/// Initialiaze a stream from a byte array.
	/// Used for deserilaizing a byte array
	/// </summary>
	/// <param name="ByteArray"></param>
	public UnitySerializer(byte[] ByteArray)
	{
		byteArray = ByteArray;
		byteStream = new List<byte>(ByteArray);
	}



	// --- double ---
	public void Serialize(double d)
	{
		byteStream.AddRange( BitConverter.GetBytes(d));

	}

	public double DeserializeDouble()
	{
		double d = BitConverter.ToDouble(ByteArray, index); index += 8;
		return d;
	}
	//

	// --- bool ---
	public void Serialize(bool b)
	{
		byteStream.AddRange(BitConverter.GetBytes(b));
	}

	public bool DeserializeBool()
	{
		bool b = BitConverter.ToBoolean(ByteArray, index); index += 1;
		return b;
	}
	//

	// --- Vector2 ---
	public void Serialize(Vector2 v)
	{
		byteStream.AddRange(GetBytes(v));
	}

	public Vector2 DeserializeVector2()
	{
		Vector2 vector2 = new Vector2();
		vector2.x = BitConverter.ToSingle(ByteArray, index); index += 4;
		vector2.y = BitConverter.ToSingle(ByteArray, index); index += 4;
		return vector2;
	}
	//

	// --- Vector3 ---
	public void Serialize(Vector3 v)
	{
		byteStream.AddRange(GetBytes(v));
	}

	public Vector3 DeserializeVector3()
	{
		Vector3 vector3 = new Vector3();
		vector3.x = BitConverter.ToSingle(ByteArray, index); index += 4;
		vector3.y = BitConverter.ToSingle(ByteArray, index); index += 4;
		vector3.z = BitConverter.ToSingle(ByteArray, index); index += 4;
		return vector3;
	}
	//

	// --- Type ---
	public void Serialize(System.Type t)
	{
		// serialize type as string
		string typeStr = t.ToString();
		Serialize(typeStr);
	}

	public Type DeserializeType()
	{
		// type stored as string
		string typeStr = DeserializeString();
		return Type.GetType(typeStr); ;
	}
	//

	// --- String ---
	public void Serialize(string s)
	{
		// add the length as a header
		byteStream.AddRange(BitConverter.GetBytes(s.Length));
		foreach (char c in s)
			byteStream.Add((byte)c);
	}

	public string DeserializeString()
	{
		int length = BitConverter.ToInt32(ByteArray, index); index += 4;
		string s = "";
		for (int i = 0; i < length; i++)
		{
			s += (char)ByteArray[index];
			index++;
		}

		return s;
	}
	//

	// --- byte[] ---
	public void Serialize(byte[] b)
	{
		// add the length as a header
		byteStream.AddRange(BitConverter.GetBytes(b.Length));
		byteStream.AddRange(b);
	}

	public byte[] DeserializeByteArray()
	{
		int length = BitConverter.ToInt32(ByteArray, index); index += 4;
		byte[] bytes = new byte[length];
		for (int i = 0; i < length; i++)
		{
			bytes[i] = ByteArray[index];
			index++;
		}

		return bytes;
	}
	//

	// --- Quaternion ---
	public void Serialize(Quaternion q)
	{
		byteStream.AddRange(GetBytes(q));
	}

	public Quaternion DeserializeQuaternion()
	{
		Quaternion quat = new Quaternion();
		quat.x = BitConverter.ToSingle(ByteArray, index); index += 4;
		quat.y = BitConverter.ToSingle(ByteArray, index); index += 4;
		quat.z = BitConverter.ToSingle(ByteArray, index); index += 4;
		quat.w = BitConverter.ToSingle(ByteArray, index); index += 4;
		return quat;
	}
	//

	// --- float ---
	public void Serialize(float f)
	{
		byteStream.AddRange(BitConverter.GetBytes(f));
	}

	public float DeserializeFloat()
	{
		float f = BitConverter.ToSingle(ByteArray, index); index += 4;
		return f;
	}
	//

	// --- int ---
	public void Serialize(int i)
	{
		byteStream.AddRange(BitConverter.GetBytes(i));
	}

	public int DeserializeInt()
	{
		int i = BitConverter.ToInt32(ByteArray, index); index += 4;
		return i;
	}
	//

	// --- internal ----
	Vector3 DeserializeVector3(byte[] bytes, ref int index)
	{
		Vector3 vector3 = new Vector3();
		vector3.x = BitConverter.ToSingle(bytes, index); index += 4;
		vector3.y = BitConverter.ToSingle(bytes, index); index += 4;
		vector3.z = BitConverter.ToSingle(bytes, index); index += 4;

		return vector3;
	}

	Quaternion DeserializeQuaternion(byte[] bytes, ref int index)
	{
		Quaternion quat = new Quaternion();
		quat.x = BitConverter.ToSingle(bytes, index); index += 4;
		quat.y = BitConverter.ToSingle(bytes, index); index += 4;
		quat.z = BitConverter.ToSingle(bytes, index); index += 4;
		quat.w = BitConverter.ToSingle(bytes, index); index += 4;
		return quat;
	}

	byte[] GetBytes(Vector2 v)
	{
		List<byte> bytes = new List<byte>(8);
		bytes.AddRange(BitConverter.GetBytes(v.x));
		bytes.AddRange(BitConverter.GetBytes(v.y));
		return bytes.ToArray();
	}

	byte[] GetBytes(Vector3 v)
	{
		List<byte> bytes = new List<byte>(12);
		bytes.AddRange(BitConverter.GetBytes(v.x));
		bytes.AddRange(BitConverter.GetBytes(v.y));
		bytes.AddRange(BitConverter.GetBytes(v.z));
		return bytes.ToArray();
	}

	byte[] GetBytes(Quaternion q)
	{
		List<byte> bytes = new List<byte>(16);
		bytes.AddRange(BitConverter.GetBytes(q.x));
		bytes.AddRange(BitConverter.GetBytes(q.y));
		bytes.AddRange(BitConverter.GetBytes(q.z));
		bytes.AddRange(BitConverter.GetBytes(q.w));
		return bytes.ToArray();
	}

	//    public static void Example()
	//    {
	//        //
	//        Debug.Log("--- UnitySerializer Example ---");
	//        Vector2 point      = UnityEngine.Random.insideUnitCircle;
	//        Vector3 position    = UnityEngine.Random.onUnitSphere;
	//        Quaternion quaternion  = UnityEngine.Random.rotation;
	//        float f         = UnityEngine.Random.value;
	//        int i          = UnityEngine.Random.Range(0, 10000);
	//        double d        = (double)UnityEngine.Random.Range(0, 10000);
	//        string s        = "Brundle Fly";
	//        bool b         = UnityEngine.Random.value < 0.5f ? true : false;
	//        System.Type type    = typeof(UnitySerializer);
	//      
	//        //
	//        Debug.Log("--- Before ---");
	//        Debug.Log(point + " " + position + " " + quaternion + " " + f + " " + d + " " + s + " " + b + " " + type);
	//      
	//        //
	//        Debug.Log("--- Serialize ---");
	//        UnitySerializer us = new UnitySerializer();
	//        us.Serialize(point);
	//        us.Serialize(position);
	//        us.Serialize(quaternion);
	//        us.Serialize(f);
	//        us.Serialize(i);
	//        us.Serialize(d);
	//        us.Serialize(s);
	//        us.Serialize(b);
	//        us.Serialize(type);
	//        byte[] byteArray = us.ByteArray;
	//      
	//        // the array must be deserialized in the same order as it was serialized
	//        Debug.Log("--- Deserialize ---");
	//        UnitySerializer uds   = new UnitySerializer(byteArray);
	//        Vector2 point2     = uds.DeserializeVector2();
	//        Vector3 position2    = uds.DeserializeVector3();
	//        Quaternion quaternion2 = uds.DeserializeQuaternion();
	//        float f2        = uds.DeserializeFloat();
	//        int i2         = uds.DeserializeInt();
	//        double d2        = uds.DeserializeDouble();
	//        string s2        = uds.DeserializeString();
	//        bool b2         = uds.DeserializeBool();
	//        System.Type type2    = uds.DeserializeType();
	//      
	//        //
	//        Debug.Log("--- After ---");
	//        Debug.Log(point2 + " " + position2 + " " + quaternion2 + " " + f2 + " " + d2 + " " + s2 + " " + b2 + " " + type2);
	//    }
}
