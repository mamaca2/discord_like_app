import { useState } from 'react'
import './App.css'
import LoginPage from './Authentication/loginpage'
import RegisterPage from './Authentication/RegisterPage'


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

  return (
    <div className="app-container">
      <header>
        <h1>Discord Clone</h1>
        <button onClick={handleLogout}>Log Out</button>
      </header>
      <main>
        <h2>Welcome to the server!</h2>
      </main>
    </div>
  )
}

export default App