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
        <input type="text" placeholder="**********" class="input input-bordered" v-model="password" />
      </div>
      <div class="form-control mt-6">
        <button class="btn btn-primary btn-outline" @click="signIn">Créer un compte</button>
        <div class="divider">OU</div>
        <button class="btn btn-primary" @click="logIn">Se connecter</button>
      </div>
    </div>
  </div>
</template>


<script setup lang="ts">

const username = ref('');
const password = ref('');


function signIn() {
  //TODO: appel a auth avec body signin

};


function logIn() {
  //TODO appel à auth avec body login
}

</script>
