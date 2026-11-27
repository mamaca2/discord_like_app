import { useState } from 'react'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import './App.css'

import LoginPage from './components/authentication/LoginPage'
import RegisterPage from './components/authentication/RegisterPage'
import MainPage from './components/MainPage'
import InvitePage from './components/chatAreas/InvitePage'

function App() {
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'))
  const [showRegister, setShowRegister] = useState(false)

  const handleLoginSuccess = (userToken: string) => {
    localStorage.setItem('token', userToken)
    setToken(userToken)
  }

  const handleLogout = () => {
    localStorage.removeItem('token')
    setToken(null)
  }

  return (
    <BrowserRouter>
      {!token ? (
        showRegister ? (
          <RegisterPage onBackToLogin={() => setShowRegister(false)} />
        ) : (
          <LoginPage
            onLoginSuccess={handleLoginSuccess}
            onGoToRegister={() => setShowRegister(true)}
          />
        )
      ) : (
        <Routes>
          <Route
            path="/"
            element={
              <MainPage
                token={token}
                onLogout={handleLogout}
              />
            }
          />

          <Route
            path="/invite/:code"
            element={<InvitePage token={token} />}
          />
        </Routes>
      )}
    </BrowserRouter>
  )
}

export default App
