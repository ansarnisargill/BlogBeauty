
JSwastika.Cms.Lib.Repositories.FileRepository.CopyDirectory(string, string)[
OG:\_github\Swastika-IO-Core\src\Swastika.Cms.Lib\Repositories\FileRepository.cs� �(srcPathdesPath"0*
0*
1
2*�
1�
[
OG:\_github\Swastika-IO-Core\src\Swastika.Cms.Lib\Repositories\FileRepository.cs�+ �(m%0JSystem.IO.Directory.GetDirectories(string, string, System.IO.SearchOption)"
	
srcPath"
"""
""*
3*
3*
4
5*�
4�
[
OG:\_github\Swastika-IO-Core\src\Swastika.Cms.Lib\Repositories\FileRepository.cs�. �(N%0string.Replace(string, string)"
	
dirPath"
	
srcPath"
	
desPath�
[
OG:\_github\Swastika-IO-Core\src\Swastika.Cms.Lib\Repositories\FileRepository.cs� �(O%1+System.IO.Directory.CreateDirectory(string)"

%0*
3*�
5�
[
OG:\_github\Swastika-IO-Core\src\Swastika.Cms.Lib\Repositories\FileRepository.cs�+ �(i%0DSystem.IO.Directory.GetFiles(string, string, System.IO.SearchOption)"
	
srcPath"
"""
""*
6*
6*
7
8*�
7�
[
OG:\_github\Swastika-IO-Core\src\Swastika.Cms.Lib\Repositories\FileRepository.cs�' �(G%0string.Replace(string, string)"
	
newPath"
	
srcPath"
	
desPath�
[
OG:\_github\Swastika-IO-Core\src\Swastika.Cms.Lib\Repositories\FileRepository.cs� �(N%1)System.IO.File.Copy(string, string, bool)"
	
newPath"

%0"
""*
6*j
8"e
[
OG:\_github\Swastika-IO-Core\src\Swastika.Cms.Lib\Repositories\FileRepository.cs� �(
""*j
2"e
[
OG:\_github\Swastika-IO-Core\src\Swastika.Cms.Lib\Repositories\FileRepository.cs� �(
""*
9"
""