import { useState } from 'react'
import LoginRegisterPage from './LoginRegister/LogingRegisterPage'
import MainPage from './MainPage/MainPage'

function App() {
    const [page, setPage] = useState<'auth' | 'main'>('auth')
    const [username, setUsername] = useState('')

    function handleLogin(username: string) {
        setUsername(username)
        setPage('main')
    }

    return (
        <>
            {page === 'auth' && (
                <LoginRegisterPage onLogin={handleLogin} />
            )}

            {page === 'main' && (
                <MainPage username={username} />
            )}
        </>
    )
}

export default App