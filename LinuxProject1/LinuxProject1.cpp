#include <stdio.h>

int main(int argc, char *argv[])
{
	printf("Hello, World! 3\n");
	{
		FILE  *file;
		file = fopen("/etc/os-release", "r");
		if (file)
		{
			char line[128];
			while (fgets(line, sizeof(line), file))
			{
				printf("%s", line);					
			}
			fclose(file);		
		}	
	}
	return 0;
}