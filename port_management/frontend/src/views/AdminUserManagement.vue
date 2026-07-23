<template>
  <div class="admin-users-container">
    <h1>Admin User Management</h1>
    <table>
      <thead>
        <tr>
          <th>Name</th><th>Email</th><th>Roles</th><th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="user in users" :key="user.id">
          <td>{{ user.name }}</td>
          <td>{{ user.email }}</td>
          <td>
            <span v-for="role in user.roles" :key="role" class="role-badge">
              {{ readableRole(role) }}
              <button class="remove-btn" @click="removeRole(user.id, role)" title="Remove this role">&times;</button>
            </span>
          </td>
          <td>
            <select v-model="newRoles[user.id]">
              <option disabled value="">Select role to assign</option>
              <option v-for="role in availableRoles" :key="role.label" :value="role.value">
                {{ role.label }}
              </option>
            </select>
            <button @click="assignRole(user.id)">Assign Role</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api"

interface User {
  id: string
  name: string
  email: string
  roles: string[]
}

const availableRoles = [
  { label: 'Administrator', value: 1 },
  { label: 'Port Authority Officer', value: 2 },
  { label: 'Shipping Agent Representative', value: 3 },
  { label: 'Logistics Operator', value: 4 }
]

const roleLabelMap: Record<string, string> = {
  1: 'Administrator',
  2: 'Port Authority Officer',
  3: 'Shipping Agent Representative',
  4: 'Logistics Operator',
  'Administrator': 'Administrator',
  'PortAuthorityOfficer': 'Port Authority Officer',
  'ShippingAgentRepresentative': 'Shipping Agent Representative',
  'LogisticsOperator': 'Logistics Operator'
}

const users = ref<User[]>([])
const newRoles = ref<Record<string, number>>({})

const fetchUsers = async () => {
  try {
    const res = await fetch(`${API_BASE}/admin/users`, {
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Failed to load users')
    users.value = await res.json()
  } catch (e) {
    alert('Error loading users')
  }
}

const readableRole = (role: string | number) => roleLabelMap[role] || role

const assignRole = async (userId: string) => {
  const roleValue = newRoles.value[userId]
  if (roleValue === undefined) {
    alert('Please select a role')
    return
  }
  try {
    const res = await fetch(`${API_BASE}/admin/users/${userId}/roles/`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({ role: roleValue })
    })
    if (!res.ok) {
      const msg = await res.text()
      throw new Error('Failed to assign role: ' + msg)
    }
    alert('Role assigned')
    await fetchUsers()
  } catch (e) {
    alert(`${e}`)
  }
}

const removeRole = async (userId: string, role: string | number) => {
  if (!confirm(`Remove role "${readableRole(role)}" from this user?`)) return
  try {
    let roleValue = role
    if (typeof role === 'string') {
      const found = availableRoles.find(r => r.label === role || r.label.replace(/ /g, '') === role)
      if (found) roleValue = found.value
    }
    const res = await fetch(`${API_BASE}/admin/users/${userId}/roles/${roleValue}`, {
      method: 'DELETE',
      credentials: 'include'
    })
    if (!res.ok) {
      const msg = await res.text()
      throw new Error('Failed to remove role: ' + msg)
    }
    alert('Role removed')
    await fetchUsers()
  } catch (e) {
    alert(`${e}`)
  }
}

onMounted(() => { fetchUsers() })
</script>

<style scoped>
.admin-users-container {
  max-width: 900px;
  margin: 40px auto;
  padding: 1.5rem;
  background: white;
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0,0,0,0.1);
}
table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}
th, td {
  padding: 0.75rem;
  border-bottom: 1px solid #ddd;
  text-align: left;
}
select {
  margin-right: 0.5rem;
  padding: 0.25rem 0.5rem;
}
button {
  padding: 0.3rem 0.8rem;
  border: none;
  background-color: #667eea;
  color: white;
  border-radius: 6px;
  cursor: pointer;
}
button:hover {
  background-color: #5866d9;
}
.role-badge {
  display: inline-block;
  background: #EFF6FF;
  color: #276EF1;
  margin-right: 6px;
  padding: 3px 8px;
  border-radius: 5px;
  font-size: 0.93em;
}
.remove-btn {
  margin-left: 5px;
  border: none;
  background: none;
  color: #e74c3c;
  font-weight: bold;
  cursor: pointer;
  font-size: 1.2em;
  line-height: 1;
}
.remove-btn:hover {
  color: #c0392b;
}
</style>
