import { useState } from "react";

function App() {
  const [msg, setMsg] = useState("");

  const test = async () => {
    try {
      const res = await fetch("http://localhost:5001/hello");
      const data = await res.text();
      setMsg(data);
    } catch (e: unknown) {
      setMsg("err");
      console.log(e);
    }
  };
  return (
    <>
      <button onClick={test}>test</button>
      {msg && <p>{msg}</p>}
    </>
  );
}

export default App;
