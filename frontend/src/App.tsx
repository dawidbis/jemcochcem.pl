import { useState,useEffect } from 'react'
import './App.css'

function App() {
  const [data, setData] = useState([])

  useEffect(() => {
    fetch('/api/food')
      .then(res => res.json())
      .then(json => setData(json))
      .catch(err => console.error("Błąd:", err))
  }, [])

  return (
    <div>
      <h1>Dane z Backend:</h1>
      <pre>{JSON.stringify(data, null, 2)}</pre>
    </div>
  )
}

export default App
