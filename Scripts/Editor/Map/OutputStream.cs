using System;

public interface OutputStream
{
	void write(int byteValue);
	void write(byte[] bytes, int offset, int length);
}
