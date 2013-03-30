#include "ImeContext.h"

namespace TouhouSpring {
namespace Ime {

ImeContext::ImeContext(System::IntPtr windowHandle)
{
    if (s_instance != nullptr)
    {
        throw gcnew System::InvalidOperationException("Only one instance of ImeContext can be created.");
    }
    s_instance = this;

    HWND hWnd = reinterpret_cast<HWND>(static_cast<void*>(windowHandle));

    ImeUiCallback_DrawRect = NULL;
    ImeUiCallback_Malloc = malloc;
    ImeUiCallback_Free = free;
    ImeUiCallback_DrawFans = NULL;

    m_initialized = ImeUi_Initialize(hWnd, false);

    //s_CompString.SetBufferSize( MAX_COMPSTRING_SIZE );
    ImeUi_EnableIme( true );

    m_wndProc = gcnew WndProcDelegate(this, &ImeContext::WindowProcedure);
    System::IntPtr funcPtr = System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegate(m_wndProc);
    m_oldWndProc = reinterpret_cast<WNDPROC>(::SetWindowLongPtr(hWnd, GWLP_WNDPROC, reinterpret_cast<LONG>(funcPtr.ToPointer())));
}

ImeContext::~ImeContext()
{
    ImeUi_Uninitialize();
}

void ImeContext::BeginIme()
{
    ImeUi_EnableIme(true);
}

void ImeContext::EndIme()
{
    ImeUi_FinalizeString();
    ImeUi_EnableIme(false);
}

LRESULT ImeContext::WindowProcedure(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    bool overrideDefault = StaticMsgProc(hWnd, msg, wParam, lParam);
    if (overrideDefault)
    {
        return 0;
    }

    ImeUi_ProcessMessage( hWnd, msg, wParam, lParam, &overrideDefault );
    if (overrideDefault)
    {
        return 0;
    }

    if (msg == WM_CHAR)
    {
        OnChar(static_cast<WCHAR>(wParam));
    }

    // Default message processing
    LRESULT retCode = ::CallWindowProc(m_oldWndProc, hWnd, msg, wParam, lParam);

    if (msg == WM_GETDLGCODE)
    {
        retCode |= DLGC_WANTALLKEYS | DLGC_WANTCHARS;
    }

    return retCode;
}

bool ImeContext::StaticMsgProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    if( !ImeUi_IsEnabled() )
        return false;

#if defined(DEBUG) || defined(_DEBUG)
    //m_bIMEStaticMsgProcCalled = true;
#endif

    switch( uMsg )
    {
    case WM_INPUTLANGCHANGE:
        System::Diagnostics::Debug::WriteLine("WM_INPUTLANGCHANGE");
        return true;

    case WM_IME_SETCONTEXT:
        System::Diagnostics::Debug::WriteLine("WM_IME_SETCONTEXT");
        //
        // We don't want anything to display, so we have to clear this
        //
        lParam = 0;
        return false;

        // Handle WM_IME_STARTCOMPOSITION here since
        // we do not want the default IME handler to see
        // this when our fullscreen app is running.
    case WM_IME_STARTCOMPOSITION:
        System::Diagnostics::Debug::WriteLine("WM_IME_STARTCOMPOSITION");
        //ResetCompositionString();
        // Since the composition string has its own caret, we don't render
        // the edit control's own caret to avoid double carets on screen.
        //s_bHideCaret = true;
        return true;
    case WM_IME_ENDCOMPOSITION:
        System::Diagnostics::Debug::WriteLine("WM_IME_ENDCOMPOSITION");
        //s_bHideCaret = false;
        return false;
    case WM_IME_COMPOSITION:
        System::Diagnostics::Debug::WriteLine("WM_IME_COMPOSITION");
        return false;
    }

    return false;
}

}
}
