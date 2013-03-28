#include "ImeContext.h"
#include "ImeUi.h"

namespace TouhouSpring {
namespace Ime {

ImeContext::ImeContext(System::IntPtr windowHandle)
{
    HWND hWnd = reinterpret_cast<HWND>(static_cast<void*>(windowHandle));

    ImeUiCallback_DrawRect = NULL;
    ImeUiCallback_Malloc = malloc;
    ImeUiCallback_Free = free;
    ImeUiCallback_DrawFans = NULL;

    m_initialized = ImeUi_Initialize(hWnd, false);

    //s_CompString.SetBufferSize( MAX_COMPSTRING_SIZE );
    ImeUi_EnableIme( true );
}

ImeContext::~ImeContext()
{
}

}
}
