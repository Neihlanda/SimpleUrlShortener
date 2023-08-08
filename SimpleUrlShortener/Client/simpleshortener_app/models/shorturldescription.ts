export interface ShortUrlDescription {
  id: string;
  destinationUrl: string;
  scrappedTitle: string | undefined;
  scrappedDescription: string | undefined;
  accessCount: number;
  expiredOn: Date;
  uniqueAccess: boolean;
}
