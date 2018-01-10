#include <stdio.h>
#include <Windows.h>
#include <iostream>
#include "com_pronto_omni_jni_JniDemo.h"
using namespace std;

JNIEXPORT void JNICALL Java_com_pronto_omni_jni_JniDemo_folderWatch
  (JNIEnv * env, jobject obj, jstring str)
{
	wchar_t *folderName =(wchar_t *)env->GetStringChars(str,0); 
    HANDLE hand = CreateFile(   
        folderName,              
        FILE_LIST_DIRECTORY, 
        FILE_SHARE_READ| FILE_SHARE_WRITE | FILE_SHARE_DELETE,
        NULL,
        OPEN_EXISTING,
        FILE_FLAG_BACKUP_SEMANTICS,
        NULL
        );
	env->ReleaseStringChars(str,(const unsigned short *)folderName); 

    char   notify[1024] = {0};   
    FILE_NOTIFY_INFORMATION   *pnotify=(FILE_NOTIFY_INFORMATION   *)notify;   
    DWORD   cbBytes;   
    while(true)   
    {   
        BOOL ret = ReadDirectoryChangesW(hand,
            &notify,   
            sizeof(notify),   
            TRUE,   
            FILE_NOTIFY_CHANGE_LAST_WRITE|FILE_NOTIFY_CHANGE_FILE_NAME,
            &cbBytes,   
            NULL,   
            NULL);

        if(ret)   
        {   
            char   AnsiChar[1024] = {0};
            WideCharToMultiByte(CP_ACP,0,pnotify->FileName,pnotify->FileNameLength/2,AnsiChar,1024,NULL,NULL);
            if(FILE_ACTION_MODIFIED   ==   pnotify->Action)   
            {
                //printf("%s Modified\n",AnsiChar);
				actionNotify(env, obj, AnsiChar, "Modified");
            }
            else if (FILE_ACTION_ADDED == pnotify->Action)
            {
                //printf("%s Add\n",AnsiChar);
				actionNotify(env, obj, AnsiChar, "Added");
            }
            else if (FILE_ACTION_REMOVED == pnotify->Action)
            {
                //printf("%s Removed\n",AnsiChar);
				actionNotify(env, obj, AnsiChar, "Removed");
            }
        }
    }
}

void actionNotify(JNIEnv * env, jobject obj, char *fileName, char *action)
{
	jclass cls = env->GetObjectClass(obj);
	jmethodID java_mthod = env->GetMethodID(cls,"fileChange","(Ljava/lang/String;Ljava/lang/String;)V");
	//(Ljava/lang/String;Ljava/lang/String;)V
	jstring argv[2];
	argv[0] = env->NewStringUTF(fileName);
	argv[1] = env->NewStringUTF(action);
	env->CallObjectMethod(obj,java_mthod,argv[0],argv[1]);
}