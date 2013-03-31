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

    HWND hWnd = reinterpret_cast<HWND>(windowHandle.ToPointer());

    m_wndProc = gcnew WndProcDelegate(this, &ImeContext::WindowProcedure);
    System::IntPtr funcPtr = System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegate(m_wndProc);
    m_oldWndProc = reinterpret_cast<WNDPROC>(::SetWindowLongPtr(hWnd, GWLP_WNDPROC, reinterpret_cast<LONG>(funcPtr.ToPointer())));

    m_imeOnInputLangChange = gcnew InputLangChangeDelegate(this, &ImeContext::ImeOnInputLangChange);
    funcPtr = System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegate(m_imeOnInputLangChange);
    typedef void (CALLBACK *ImeOnInputLangChangeCallback)();
    ImeUiCallback_OnInputLangChange = reinterpret_cast<ImeOnInputLangChangeCallback>(funcPtr.ToPointer());

    ImeUiCallback_DrawRect = NULL;
    ImeUiCallback_Malloc = malloc;
    ImeUiCallback_Free = free;
    ImeUiCallback_DrawFans = NULL;

    m_initialized = ImeUi_Initialize(hWnd, true, false);
    if (!m_initialized)
    {
        return;
    }

    ImeUi_EnableIme( true );
}

ImeContext::~ImeContext()
{
    ImeUi_Uninitialize();
}

System::String^ ImeContext::IndicatorString::get()
{
    return gcnew System::String(ImeUi_IsEnabled() ? ImeUi_GetIndicatior() : L"En");
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

    if (m_initialized)
    {
        switch (msg)
        {
        case WM_IME_COMPOSITION:
            {
                System::String^ compStr = gcnew System::String(ImeUi_GetCompositionString());
                cli::array<ClauseAttribute>^ attrArray = gcnew cli::array<ClauseAttribute>(compStr->Length);
                BYTE* attr = ImeUi_GetCompStringAttr();
                assert(attr != NULL);
                for (int i = 0; i < compStr->Length; ++i)
                {
                    attrArray[i] = safe_cast<ClauseAttribute>(attr[i]);
                }
                OnComposition(compStr, attrArray, ImeUi_GetImeCursorChars());
            }
            break;
        case WM_IME_ENDCOMPOSITION:
            OnEndComposition();
            break;
        default:
            break;
        }
    }
    if (overrideDefault)
    {
        return 0;
    }

    if (msg == WM_CHAR)
    {
        OnChar(static_cast<WCHAR>(wParam));
    }
    else if (msg == WM_KEYDOWN)
    {
        OnKeyDown(static_cast<WCHAR>(wParam));
    }
    else if (msg == WM_KEYUP)
    {
        OnKeyUp(static_cast<WCHAR>(wParam));
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
        //System::Diagnostics::Debug::WriteLine("WM_INPUTLANGCHANGE");
        return true;

    case WM_IME_SETCONTEXT:
        //System::Diagnostics::Debug::WriteLine("WM_IME_SETCONTEXT");
        //
        // We don't want anything to display, so we have to clear this
        //
        lParam = 0;
        return false;

        // Handle WM_IME_STARTCOMPOSITION here since
        // we do not want the default IME handler to see
        // this when our fullscreen app is running.
    case WM_IME_STARTCOMPOSITION:
        //System::Diagnostics::Debug::WriteLine("WM_IME_STARTCOMPOSITION");
        //ResetCompositionString();
        // Since the composition string has its own caret, we don't render
        // the edit control's own caret to avoid double carets on screen.
        //s_bHideCaret = true;
        return true;
    case WM_IME_ENDCOMPOSITION:
        //System::Diagnostics::Debug::WriteLine("WM_IME_ENDCOMPOSITION");
        //s_bHideCaret = false;
        return false;
    case WM_IME_COMPOSITION:
        //System::Diagnostics::Debug::WriteLine("WM_IME_COMPOSITION");
        return false;
    }

    return false;
}

void ImeContext::ImeOnInputLangChange()
{
    OnInputLangChange(IndicatorString);
}

}
}
