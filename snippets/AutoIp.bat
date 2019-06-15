echo off
cls

echo 设置IP

netsh interface ip set address "本地连接" static 192.168.1.11 255.255.255.0 192.168.1.254 1

echo 设置主DNS

netsh interface ip set dns name="本地连接" static 192.168.16.253 primary

echo ******恭喜你，修改完成！******

pause