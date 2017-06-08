using System;
using System.IO;

public class FileOutputStream : OutputStream
{
	// フィールド
	private FileStream m_data;
	
	
	// コンストラクタ
	public FileOutputStream(FileStream fs)
	{
		m_data = fs;
	}

	// インターフェースメソッド
	public void write(int byteValue)
	{
		m_data.WriteByte((byte)byteValue);
	}
	
	// インターフェースメソッド
	public void write(byte[] bytes, int offset, int length)
	{
		m_data.Write(bytes, offset, length);
	}
	
	// 
	public FileStream getFileStream()
	{
		return m_data;
	}

}
