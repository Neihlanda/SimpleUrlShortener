export interface ErrorDetails {
  messages: string[];
  source: string | undefined;
  exception: string | undefined;
  statusCode: number;
}
