using System;

public class DataOutputStream : OutputStream
{

	protected OutputStream o_stream;
	
	// バイトオーダーの変更
	private bool _is_reverse;	// 初期値はtrue
		
	// コンストラクタ
	public DataOutputStream(OutputStream o_stm) {
		
		o_stream = o_stm;
		_is_reverse = true;
		
	} // DataOutputStream
	
	public void write(int data) {
		
		o_stream.write(data);
		
	} // write
	
	public void write(byte[] bytes, int offset, int length) {
		
		o_stream.write(bytes, offset, length);
		
	} // write
	
	public void writeBool(bool value) {
		
		o_stream.write(value? 1: 0);
		
	} // writeByte
	
	public void writeByte(byte value) {
		
		o_stream.write(value);
		
	} // writeByte
	
	public void writeShort(short value) {
		
		int size = sizeof(short);
		byte[] tmp = BitConverter.GetBytes(value);
		if (_is_reverse) reverse(tmp, size);
		o_stream.write(tmp, 0, size);
		
	} // writeShort
	
	public void writeInt(int value) {
		
		int size = sizeof(int);
		byte[] tmp = BitConverter.GetBytes(value);
		if (_is_reverse) reverse(tmp, size);
		o_stream.write(tmp, 0, size);
		
	} // writeInt
	
	public void writeFloat(float value) {
		
		int size = sizeof(float);
		byte[] tmp = BitConverter.GetBytes(value);
		if (_is_reverse) reverse(tmp, size);
		o_stream.write(tmp, 0, size);
		
	} // writeFloat
	
	public void writeLongLong(Int64 value) {
		
		int size = sizeof(Int64);
		byte[] tmp = BitConverter.GetBytes(value);
		if (_is_reverse) reverse(tmp, size);
		o_stream.write(tmp, 0, size);
		
	} // writeLongLong
	
	public void writeDouble(double value) {
		
		int size = sizeof(double);
		byte[] tmp = BitConverter.GetBytes(value);
		if (_is_reverse) reverse(tmp, size);
		o_stream.write(tmp, 0, size);
		
	} // writeDouble
	
	public void writeUtf8Len1(string text)
	{
		byte[] bin = System.Text.UTF8Encoding.UTF8.GetBytes(text);
		int length = Math.Min(255, bin.Length);
		writeByte((byte)length);
		o_stream.write(bin, 0, length);
	}
	
	public void setReverse(bool rev) {
		
		_is_reverse = rev;
		
	} // setReverse
	
	private void reverse(byte[] buffer, int length) {
		
		// バイトオーダーを逆にする
		int low = 0;
		int high = length;
		byte temp;
		
		while(--high > low) {
			temp = buffer[low];
			buffer[low] = buffer[high];
			low++;
			buffer[high] = temp;
		}
		
	} // reverse
	
}

