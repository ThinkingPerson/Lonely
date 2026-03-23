@echo off

rem 停止nginx服务
echo 正在停止nginx服务...
"d:\lizhihang\TraeProject\Lonely\Push\nginx-1.28.2\nginx.exe" -s stop

rem 等待2秒让nginx完全停止
ping 127.0.0.1 -n 3 > nul

rem 启动nginx服务
echo 正在启动nginx服务...
"d:\lizhihang\TraeProject\Lonely\Push\nginx-1.28.2\nginx.exe"

echo nginx服务重启完成！
pause