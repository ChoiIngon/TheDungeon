public struct Tuple <T1, T2> {
	public readonly T1 first;
	public readonly T2 second;

	public Tuple(T1 item1, T2 item2) {
		first = item1;
		second = item2;
	}
}