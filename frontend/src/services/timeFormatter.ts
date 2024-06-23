export default function formattedTime(time: number): string {
  return (time / 1000).toFixed(0).toString();
  // const minutes = (time / 60000).toFixed(0);
  // const seconds = ((time % 60000) / 1000).toFixed(0);
  // return `${minutes}:${seconds}`;
}
