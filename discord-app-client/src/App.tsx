import { useState } from 'react'
import './App.css'
import LoginPage from './Authentication/LoginPage'
import RegisterPage from './Authentication/RegisterPage'
import MainPage from './Main/MainPage' 

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