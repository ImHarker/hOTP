namespace hOTPcommon {
	public class Code {
		public string Value { get; }
		public long TimeRemaining { get; set; }

		public Code(string value, long timeRemaining) {
			Value = value;
			TimeRemaining = timeRemaining;
		}

		public override string ToString() {
			return $"{Value} ({TimeRemaining:D2}s)";
		}
	}
}
