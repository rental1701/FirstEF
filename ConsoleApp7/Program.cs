// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
int[] a = { 18811, 13566 };
byte[] x = BitConverter.GetBytes(a[0]);
byte[] y = BitConverter.GetBytes(a[1]);
byte[] c = AddByteToArray(x, y);
float r = BitConverter.ToSingle(c.Reverse().ToArray(), 0);	
Console.WriteLine(r);


  byte[] AddByteToArray(byte[] a, byte[] b)
{
    byte[] res = new byte[a.Length + b.Length];
	int index = 0;
	for (; index < a.Length; index++)
	{
		res[index] = a[index];
	}
	for (; index < b.Length; index++)
	{
        res[index] = b[index];
    }
	return res;
}
