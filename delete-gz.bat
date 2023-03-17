@echo off
setlocal

set "target_dir=./docs/"

rem 递归删除.gz结尾的文件
for /r "%target_dir%" %%f in (*.gz) do (
  echo Deleting "%%f"
  del /f /q "%%f"
)

echo All done!
