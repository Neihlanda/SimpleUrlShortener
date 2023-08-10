<template>
  <div class="grid card bg-base-300 rounded-box place-items-center">
    <h1 class="text-3xl font-bold">Retrouvez votre historique</h1>
    <div class="card-body">
      <div class="form-control">
        <label class="label">
          <span class="label-text">Identifiant</span>
        </label>
        <input type="text" placeholder="Identifiant" class="input input-bordered" v-model="username" />
      </div>
      <div class="form-control">
        <label class="label">
          <span class="label-text">Mot de passe</span>
        </label>
        <input type="password" placeholder="**********" class="input input-bordered" v-model="password" />
      </div>
      <div class="form-control mt-6">
        <button class="btn btn-primary btn-outline" @click="register" :disabled="isFetching">
          Créer un compte
        </button>
        <div class="divider">OU</div>
        <button class="btn btn-primary" @click="logIn" :disabled="isFetching">
          Se connecter
        </button>
      </div>
    </div>
  </div>
</template>


<script setup lang="ts">
import { useToast } from 'vue-toast-notification';
import { ErrorDetails } from 'models';

const account = useAccountStore();

const username = ref('');
const password = ref('');
const isFetching = ref(false);
const $toast = useToast();

function register() {
  if(isFetching.value) return;

  isFetching.value = true;
  account
    .registerUser(username.value, password.value)
    .catch((error: ErrorDetails) => {
      error?.messages?.forEach(p =>
        $toast.error(p)
        )
    })
    .finally(() => isFetching.value = false);
};


function logIn() {
  if(isFetching.value) return;
  isFetching.value = true;
  account
    .login(username.value, password.value)
    .catch((error: ErrorDetails) => {
      error?.messages?.forEach(p =>
        $toast.error(p)
        )
    })
    .finally(() => isFetching.value = false);
}

</script>
