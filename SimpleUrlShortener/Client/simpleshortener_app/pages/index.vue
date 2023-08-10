<!-- eslint-disable vue/multi-word-component-names -->
<template>
  <div v-if="!isInit" class="flex flex-col justify-center items-center">
    <span class="loading loading-spinner text-primary "></span>
  </div>
  <div v-else>
    <RedirectToDestinationUrlLoader v-if="shortUrlDescriptionToRedirect !== undefined"
      :url-description="shortUrlDescriptionToRedirect" />
    <template v-else>
      <div class="flex flex-col justify-center items-center" v-cloack>
        <div class="w-full">
          <ShortenizeUrlForm />
        </div>
        <div class="divider"></div>
        <span class="loading loading-spinner text-primary " v-if="isFetchingUser"></span>
        <div class="w-full" v-else>
          <component :is="isLoggedIn ? UrlShortenedHistory : SignInLogInForm" />
        </div>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, useRoute, computed } from '#imports';
import { ShortUrlDescription } from '../models/shorturldescription';
import { UrlShortenedHistory, SignInLogInForm } from '#components'
import { ErrorDetails } from 'models';

const route = useRoute();
const account = useAccountStore();
const urlshortenerStore = useShortenedUrlStore();
const shortUrlDescriptionToRedirect = ref<ShortUrlDescription>();
const isLoggedIn = computed(() => account.loggedIn)
const isFetchingUser = computed(() => account.isFetching)

const { $toast } = useNuxtApp();
const isInit = ref(false);

onMounted(async () => {
  try {
    await account.initStore();
    urlshortenerStore.initStore();
    await loadUrlByQueryParamters();
  }
  finally {
    isInit.value = true
  }
});

watch(() => route.query.id, async () => {
  if (!isInit.value) return
  shortUrlDescriptionToRedirect.value = undefined;
  await loadUrlByQueryParamters();
});

async function loadUrlByQueryParamters() {
  if (route.query.id && route.query.id.length >= 0) {
    if (Array.isArray(route.query.id) && route.query.id[0])
      await loadUrlDescription(route.query.id[0].toString());
    else
      await loadUrlDescription(route.query.id.toString());
  }
}

async function loadUrlDescription(urlDescriptionId: string) {
  return urlshortenerStore.getShortUrlDescription(urlDescriptionId).then(data => {
    if (data)
      shortUrlDescriptionToRedirect.value = data;
  }).catch((error: ErrorDetails) => {
    error?.messages?.forEach(p =>
      $toast.error(p)
    )
  });
}
</script>
