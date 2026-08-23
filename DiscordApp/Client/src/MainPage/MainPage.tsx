type MainPageProps = {
    username: string
}

function MainPage({ username }: MainPageProps) {
    return (
        <div>
            <h1>Discord App</h1>

            <p>Logged in as: {username}</p>
        </div>
    )
}

export default MainPage