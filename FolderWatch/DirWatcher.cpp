#define _WIN32_WINNT 0x0500

#include <Winbase.h>

 

#include <string>

#include <cassert>

#include <conio.h>

#include <iostream>

using namespace std;

 

enum ACTION { ADDED=1, REMOVED=2, MODIFIED=3, RENAMED=4 };//�ж��ļ��������͵�ö��

/*

*���������Ϣ�ú���

*/

void __stdcall MyDeal( ACTION act, std::string filename1, std::string filename2 )

{

    switch( act )

    {

    case ADDED:

        cout << "Added    - " << filename1 << endl;

        break;

    case REMOVED:

        cout << "Removed  - " << filename1 << endl;

        break;

    case MODIFIED:

        cout << "Modified - " << filename1 << endl;

        break;

    case RENAMED:

        cout << "Rename   - " << filename1 << " " << filename2 << endl;

        break;

    }

};

class FileSystemWatcher

{

public:

    typedef void (__stdcall *LPDEALFUNCTION)( ACTION act, std::string filename1, std::string filename2 );

 

    bool Run( std::string path)

    {

        WatchedDir = path;

       

        DealFun = MyDeal;

       

        DWORD ThreadId;

              cout<<"�����߳�"<<endl;

        hThread=CreateThread( NULL,0,Routine,this,0,&ThreadId );

              cout<<"��������"<<endl;

              if (NULL!=hThread) {

                     cout<<"�����̳߳ɹ�"<<endl;

              } else {

                     cout<<"�����߳�ʧ��"<<endl;

              }

        return NULL!=hThread;

}

 

//�ͷ���Դ

    void Close()

    {

        if( NULL != hThread )

        {

            TerminateThread( hThread, 0 );

            hThread = NULL;

        }

        if( INVALID_HANDLE_VALUE != hDir )

        {

            CloseHandle( hDir );

            hDir = INVALID_HANDLE_VALUE;

        }

    }

    FileSystemWatcher() : DealFun(NULL), hThread(NULL), hDir(INVALID_HANDLE_VALUE)

    {

    }

    ~FileSystemWatcher()

    {

        Close();

    }

 

private:

    std::string WatchedDir;

    LPDEALFUNCTION DealFun;

    HANDLE hThread;

    HANDLE hDir;

private:

    FileSystemWatcher( const FileSystemWatcher& );

    FileSystemWatcher operator=( const FileSystemWatcher );

private:

    static DWORD WINAPI Routine( LPVOID lParam )

    {

              cout<<"��ʼִ���߳�"<<endl;

        FileSystemWatcher* obj = (FileSystemWatcher*)lParam;

              cout<<"Ҫ�۲��Ŀ¼"<<obj->WatchedDir<<endl;

              cout<<direc<<endl;

        obj->hDir = CreateFile(

            obj->WatchedDir.c_str(),

            GENERIC_READ|GENERIC_WRITE,

            FILE_SHARE_READ|FILE_SHARE_WRITE|FILE_SHARE_DELETE,

            NULL,

            OPEN_EXISTING,

            FILE_FLAG_BACKUP_SEMANTICS,

            NULL

        );

                     cout<<"�����ļ�"<<endl;

                     if( INVALID_HANDLE_VALUE == obj->hDir ) {

                            cout<<"��Чֵ"<<endl;

                            return false;

                     }

        char buf[ 2*(sizeof(FILE_NOTIFY_INFORMATION)+MAX_PATH) ];

        FILE_NOTIFY_INFORMATION* pNotify=(FILE_NOTIFY_INFORMATION *)buf;

        DWORD BytesReturned;

cout<<"��ȡ��Ϣ"<<endl;

        while(true)

        {

                     

            if( ReadDirectoryChangesW( obj->hDir,

                pNotify,

                sizeof(buf),

                true,

                FILE_NOTIFY_CHANGE_FILE_NAME|

                FILE_NOTIFY_CHANGE_DIR_NAME|

                FILE_NOTIFY_CHANGE_ATTRIBUTES|

                FILE_NOTIFY_CHANGE_SIZE|

                FILE_NOTIFY_CHANGE_LAST_WRITE|

                FILE_NOTIFY_CHANGE_LAST_ACCESS|

                FILE_NOTIFY_CHANGE_CREATION|

                FILE_NOTIFY_CHANGE_SECURITY,

                &BytesReturned,

                NULL,

                NULL ) )

            {

                            cout<<"��ȡ��Ϣ�ɹ�"<<endl;

                char tmp[MAX_PATH], str1[MAX_PATH], str2[MAX_PATH];

                memset( tmp, 0, sizeof(tmp) );

                WideCharToMultiByte( CP_ACP,0,pNotify->FileName,pNotify->FileNameLength/2,tmp,99,NULL,NULL );

                strcpy( str1, tmp );

 

                if( 0 != pNotify->NextEntryOffset )

                {

                    PFILE_NOTIFY_INFORMATION p = (PFILE_NOTIFY_INFORMATION)((char*)pNotify+pNotify->NextEntryOffset);

                    memset( tmp, 0, sizeof(tmp) );

                    WideCharToMultiByte( CP_ACP,0,p->FileName,p->FileNameLength/2,tmp,99,NULL,NULL );

                    strcpy( str2, tmp );

                }

 

                obj->DealFun( (ACTION)pNotify->Action, str1, str2 );

            }

            else

            {

                  cout<<"��ȡ��Ϣʧ��"<<endl;

                break;

            }

        }

 

        return 0;

    }

};