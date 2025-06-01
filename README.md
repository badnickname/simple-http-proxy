# Simple Proxy
Незамысловатый HTTP-прокси, основная задумка которого в поддержке [PAC](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Proxy_servers_and_tunneling/Proxy_Auto-Configuration_PAC_fil) для настройки 
## Как запустить
Поддерживается только Windows (остальные сборки будут настроены позже)
### cmd
```bash
./SimpleProxy.exe --port 1234 --pac pac.js
```

### pac.js
Концептуально PAC-файл повторяет реализацию в браузерах: [см. developer.mozilla.org](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Proxy_servers_and_tunneling/Proxy_Auto-Configuration_PAC_file). Но на данный момент все возможности не реализованы
```js
function FindProxyForURL(url, host) {
    // для всех ресурсов, содержащих 'some-resource' в hostname, запрос пойдет через прокси 'localhost:1080'
    if (host.includes('some-resource')) {
        return 'PROXY localhost:1080'
    // иначе запрос будет отправлен напрямую
    } else {
        return 'DIRECT'
    }
}
```