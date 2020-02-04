<template>
  <div id="app">
    <v-app id="inspire">
      <v-app-bar color="primary"
                 dark
                 fixed
                 app>
        <v-btn icon @click.stop="$router.push('/')">
          <v-icon>mdi-email-mark-as-unread</v-icon>
        </v-btn>
        <v-toolbar-title>
          <router-link :to="'/'" class="appTitle">
            Справочник «Группы рассылки»
          </router-link>
        </v-toolbar-title>
        <v-spacer></v-spacer>
        <v-btn icon @click.stop="right = !right">
          <v-icon>mdi-menu</v-icon>
        </v-btn>
      </v-app-bar>
      <v-content>
        <div class="pageWrap">
          <div class="leftPart">
            <app-tree :showDeletedItems="showDeletedItems" :treeReloadProperty="$store.state.treeReloadProperty" :isMain="isMain" @changeNode="changeNode($event)"></app-tree>
          </div>
          <div class="rightPart">
            <router-view></router-view>
          </div>
        </div>
      </v-content>
      <v-navigation-drawer right
                           temporary
                           v-model="right"
                           fixed>

        <v-list-item>
          <v-list-item-content>
            <v-list-item-title class="title">
              Администрирование
            </v-list-item-title>
            <v-list-item-subtitle>
              справочника «Группы рассылки»
            </v-list-item-subtitle>
          </v-list-item-content>
        </v-list-item>

        <v-divider></v-divider>

        <v-list dense
                nav>
          <v-list-item v-for="item in items"
                       :key="item.title"
                       link
                       router
                       :to="item.action"
                       class="menu-point"
                       v-if="hasAllRights">
            <v-list-item-icon>
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-item-icon>

            <v-list-item-content>
              <v-list-item-title>{{ item.title }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
          <v-divider v-if="hasAllRights"></v-divider>
          <v-checkbox v-model="showDeletedItems"
                      label="Удалённые объекты"
                      class="fs14"></v-checkbox>
        </v-list>
      </v-navigation-drawer>
    </v-app>
  </div>
</template>

<script>
  export default {
    data: () => ({
      items: [
          { title: 'Домашняя', icon: 'mdi-home', action: "/" },
          { title: 'Добавить', icon: 'mdi-plus-circle', action: "/add" },
          { title: 'Синхронизировать', icon: 'mdi-sync', action: "/sync" },
        ],

      right: false,
      showDeletedItems: false,
      isMain: true
    }),

    methods: {
      changeNode(id) {
        if (id.length > 0) {
          //if (this.$route.path.indexOf("show") != -1) {
          //  this.$router.push({ path: '/show/' + id[0] })
          //}
          //else if (this.$route.path.indexOf("edit") != -1) {
          //  this.$router.push({ path: '/edit/' + id[0] })
          //}
          this.$router.push({ path: '/show/' + id[0] })
        }
      }
    },

    computed: {
      hasAllRights() {
        return this.$http.get(this.$store.state.baseUrl + `/api/values/HasAllRights/`)
      },
    }
  }
</script>
