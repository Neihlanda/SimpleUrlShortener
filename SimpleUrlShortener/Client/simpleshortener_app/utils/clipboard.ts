export function copyToClipBoard(text: string) {
  if (navigator.clipboard !== void 0) {
    return navigator.clipboard.writeText(text);
  }
  return Promise.reject();
}
