import { ErrorDetails, LoginRequest, RegisterRequest } from 'models';
import { userClaims } from 'models/userClaims';

type storeStateType = {
  loggedIn: boolean;
  user: userClaims | undefined;
  isFetching: boolean;
};

let acccountEndpoint = '/account/';

export const useAccountStore = defineStore('accountStore', {
  state: (): storeStateType => ({
    loggedIn: false,
    user: undefined,
    isFetching: false,
  }),
  actions: {
    async initStore() {
      this.isFetching = true;
      try {
        acccountEndpoint = useRuntimeConfig().public.api + '/account/';
        await this.fetchUser();
      } finally {
        this.isFetching = false;
      }
    },
    async login(login: string, password: string) {
      const loginRequest: LoginRequest = {
        login,
        password,
      };
      this.isFetching = true;
      try {
        const { data } = await useFetch<ErrorDetails>(
          acccountEndpoint + 'login',
          {
            method: 'POST',
            body: loginRequest,
            credentials: 'include',
            ignoreResponseError: true,
          }
        );

        if (data.value) return Promise.reject(data.value);

        await this.fetchUser();
        return true;
      } finally {
        this.isFetching = false;
      }
    },
    async logout() {
      this.user = undefined;
      this.loggedIn = this.user !== undefined;

      this.isFetching = true;
      $fetch(acccountEndpoint + 'logout', {
        method: 'POST',
        credentials: 'include',
      }).finally(() => (this.isFetching = false));
    },
    async fetchUser() {
      try {
        const data = await $fetch<userClaims>(acccountEndpoint + 'me', {
          credentials: 'include',
        });
        if (data != null && Object.keys(data).length !== 0) {
          this.user = data;
        }
      } finally {
        this.loggedIn = this.user !== undefined;
      }
    },
    async registerUser(login: string, password: string) {
      const registerRequest: RegisterRequest = {
        login,
        password,
        confirmPassword: password,
      };
      this.isFetching = true;
      try {
        const { data } = await useFetch<ErrorDetails>(
          acccountEndpoint + 'register',
          {
            method: 'POST',
            body: registerRequest,
            credentials: 'include',
            ignoreResponseError: true,
          }
        );

        if (data.value) return Promise.reject(data.value);

        await this.fetchUser();
        return true;
      } finally {
        this.isFetching = false;
      }
    },
  },
});
