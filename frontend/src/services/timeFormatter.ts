class TimeFormatter {
  static formatAsSeconds(time: number, decimal: number): string {
    return (time / 1000).toFixed(decimal).toString();
  }

  static formatAsMinutesAndSeconds(time: number): string {
    const minutes = Math.floor(time / 60000).toString();
    const seconds = ((time % 60000) / 1000).toFixed(0).padStart(2, "0");
    return `${minutes}:${seconds}`;
  }
}

export default TimeFormatter;
