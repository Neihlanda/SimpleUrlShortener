import {
  CreateShortUrlRequest,
  ErrorDetails,
  ShortUrlDescription,
} from 'models';

type storeStateType = {
  history: ShortUrlDescription[];
  isFetching: boolean;
};

let urlshortnerEndpoint = '/urlshortener/';

export const useShortenedUrlStore = defineStore('shortenedUrl', {
  state: (): storeStateType => ({
    history: [],
    isFetching: false,
  }),
  actions: {
    initStore() {
      urlshortnerEndpoint = useRuntimeConfig().public.api + '/urlshortener/';
    },
    async fetchHistory() {
      const accountStore = useAccountStore();
      if (!accountStore.$state.loggedIn) return;

      this.isFetching = true;
      try {
        const { data } = await useFetch<ShortUrlDescription[] | ErrorDetails>(
          urlshortnerEndpoint + 'createdbyme',
          {
            credentials: 'include',
            ignoreResponseError: true,
          }
        );
        if (data.value != null && Array.isArray(data.value)) {
          this.history = data.value;
        }
      } finally {
        this.isFetching = false;
      }
    },
    async shortenizeUrl(urlToProcess: string, uniqueUsage: boolean) {
      const shortenizeRequest: CreateShortUrlRequest = {
        uniqueUsage: uniqueUsage,
        urlToProcess: urlToProcess,
      };
      this.isFetching = true;
      try {
        const { data } = await useFetch<ShortUrlDescription | ErrorDetails>(
          urlshortnerEndpoint,
          {
            method: 'POST',
            body: shortenizeRequest,
            credentials: 'include',
            ignoreResponseError: true,
          }
        );

        if (data.value && 'messages' in data.value) {
          return Promise.reject(data.value as ErrorDetails);
        } else if (data.value) {
          this.history.splice(0, 0, data.value);
        }

        return data.value;
      } finally {
        this.isFetching = false;
      }
    },
    async getShortUrlDescription(id: string) {
      try {
        this.isFetching = true;
        const { data } = await useFetch<ShortUrlDescription | ErrorDetails>(
          `${urlshortnerEndpoint}${id}`,
          {
            credentials: 'include',
            ignoreResponseError: true,
          }
        );

        if (data.value && 'messages' in data.value) {
          return Promise.reject(data.value as ErrorDetails);
        }

        return data.value as ShortUrlDescription;
      } finally {
        this.isFetching = false;
      }
    },
  },
});
