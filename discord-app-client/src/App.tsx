import { useState } from 'react'
import './App.css'
import LoginPage from './main/components/authentication/LoginPage'
import RegisterPage from './main/components/authentication/RegisterPage'
import MainPage from './main/MainPage' 

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

  if (!token) {
    return showRegister ? (
      <RegisterPage onBackToLogin={() => setShowRegister(false)} />
    ) : (
      <LoginPage
        onLoginSuccess={handleLoginSuccess}
        onGoToRegister={() => setShowRegister(true)}
      />
    )
  }

  return <MainPage token={token} onLogout={handleLogout} />
}

export default App