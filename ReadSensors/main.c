#include "stdio.h"
#include "stdlib.h"
#include "string.h"
#include "stdint.h"
#include <unistd.h>


void sensor(FILE* data,char* buffer);

int main(int argc, char* argv[])
{
	(void)argc;(void)argv;
    FILE* data = fopen("../data.bin","rb");
    char buffer[28];
    char buffer_data[58];
    char* cmd="mosquitto_pub";
    char* parametros[10]={"mosquitto_pub","-t","source","-m","","-h","10.20.141.38","-k","3",NULL};
	
    while(1){
    	sleep(10);
    	if (data==NULL)
    		continue;
  		else
  		{
    		fseek (data, 0, SEEK_END);
    		int size=ftell(data);
    		fseek(data, 0, SEEK_SET);
    		if(size!=0){
				//10 segundos - primeiros 2 sensores
				sensor(data,buffer);
				strncpy(buffer_data, buffer, sizeof buffer+1);
				sensor(data,buffer);
				strncpy(&buffer_data[29], buffer, sizeof buffer+1);


				parametros[4] = buffer_data;
				if(fork()==0){
					execvp(cmd,parametros);
					exit(0);
				}
    		}
  		}
    }
    

    //memset(buffer,0,sizeof buffer);
    fclose(data);
    return 0;
}

void sensor(FILE* data,char* buffer){
	float temperatura=0,humidade=0;
    int timestamp=0;
    uint8_t id=0,bateria=0;

	fread(&id,sizeof id, 1, data);
    fseek(data, 3, SEEK_CUR);
	fread(&temperatura,sizeof temperatura, 1, data);
	fread(&humidade,sizeof humidade, 1, data);
	fread(&bateria,sizeof bateria, 1, data);
	fseek(data, 3, SEEK_CUR);
	fread(&timestamp,sizeof timestamp, 1, data);
    fseek(data, 4, SEEK_CUR);

    /*printf("id:%d\n", id);
    printf("temperatura: %.2f\n", temperatura);
    printf("humidade: %.2f\n", humidade);
    printf("bateria: %d\n", bateria);
    printf("timestamp: %d\n", timestamp);*/

    sprintf(buffer, "%d/%.2f/%.2f/%d/%d_",id,temperatura,humidade,bateria,timestamp);
}