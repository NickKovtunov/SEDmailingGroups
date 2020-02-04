import HomePage from 'components/home-page'
import Show from 'components/show'
import Edit from 'components/edit'
import Add from 'components/add'
import Sync from 'components/sync'
import Error from 'components/error'

export const routes = [
  { name: 'home', path: '/', component: HomePage, display: 'Home', icon: 'home' },
  { name: 'show', path: '/show/:id', component: Show, display: 'Show', icon: 'info' },
  { name: 'edit', path: '/edit/:id', component: Edit, display: 'Edit', icon: 'info' },
  { name: 'add', path: '/add', component: Add, display: 'Add', icon: 'info' },
  { name: 'sync', path: '/sync', component: Sync, display: 'Sync', icon: 'info' },
  { name: 'error', path: '*', component: Error, display: 'Error', icon: 'info' },
]
