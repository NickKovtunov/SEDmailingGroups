import '@babel/polyfill'
import 'whatwg-fetch'
import Vue from 'vue'
import vuetify from './vuetify/vuetify'
import 'vuetify/dist/vuetify.min.css'
import axios from 'axios'
import router from './router/index'
import store from './store'
import { sync } from 'vuex-router-sync'
import App from 'components/app-root'
import { FontAwesomeIcon } from './icons'

import Tree from './components/tree.vue'
import DepartmentsTree from './components/departmentsTree.vue'

Vue.component('app-tree', Tree)
Vue.component('app-departmentstree', DepartmentsTree)

// Registration of global components
Vue.component('icon', FontAwesomeIcon)

Vue.filter('dateFormat', (value) => {
  var date = new Date(value);
  var year = date.getFullYear();
  var month = date.getMonth() + 1;
  month = month < 10 ? '0' + month : month;
  var day = date.getDate();
  day = day < 10 ? '0' + day : day;
  var hours = date.getHours();
  hours = hours < 10 ? '0' + hours : hours;
  var minute = date.getMinutes();
  minute = minute < 10 ? '0' + minute : minute;
  date = day + '.' + month + '.' + year + ' ' + hours + ':' + minute;
  return date;
})

Vue.filter('nameFormat', (value) => {
  name = value.slice(11);
  return name;
})

Vue.prototype.$http = axios

sync(store, router)

const app = new Vue({
  store,
  router,
  vuetify,
  ...App
})

export {
  app,
  router,
  vuetify,
  store
}

