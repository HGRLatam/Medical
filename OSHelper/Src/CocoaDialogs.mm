#include "StdAfx.h"
#include "NativeOSWindow.h"
#include "NativeDialog.h"

#include <string>

#include <Cocoa/Cocoa.h>

//Wildcard, these are in the format description|extension|description|extension
//Will return true if the filters should be used and false if not.
bool convertWildcards(const std::string& wildcard, NSMutableArray* fileTypeVector)
{
    if(wildcard.length() == 0)
    {
        return false;
    }
	size_t pos = 0;
    size_t lastPos = 0;
    size_t dotPos = 0;
	pos = wildcard.find('|');
	if(pos == std::string::npos)
	{
        dotPos = wildcard.find('.');
        pos = wildcard.length();
        if(dotPos != std::string::npos)
        {
            //Consider the whole string the filter, from the . to the end
            [fileTypeVector addObject:[NSString stringWithUTF8String:wildcard.substr(dotPos, pos - dotPos).c_str()]];
        }		
	}
	else
	{
		int pipeCount = 0;
		do
		{
			++pipeCount;
            if(pipeCount % 2 == 0)
            {
                dotPos = wildcard.find('.', lastPos);
                if(dotPos != std::string::npos && ++dotPos < pos)
                {
                    [fileTypeVector addObject:[NSString stringWithUTF8String:wildcard.substr(dotPos, pos - dotPos).c_str()]];
                }
            }
            lastPos = pos + 1;
			pos = wildcard.find('|', pos + 1);
		}
		while(pos != std::string::npos);
		//If the last character was not a pipe make sure to add the last extension
		if(wildcard.rfind('|') + 1 != wildcard.length())
		{
            pos = wildcard.length();
			++pipeCount;
            if(pipeCount % 2 == 0)
            {
                dotPos = wildcard.find('.', lastPos);
                if(dotPos != std::string::npos && ++dotPos < pos)
                {
                    [fileTypeVector addObject:[NSString stringWithUTF8String:wildcard.substr(dotPos, pos - dotPos).c_str()]];
                }
            }
		}
	}
    
    return true;
}

extern "C" _AnomalousExport void FileOpenDialog_showModal(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple, FileOpenDialogSetPathString setPathString, FileOpenDialogResultCallback resultCallback)
{    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSOpenPanel *oPanel = [NSOpenPanel openPanel];
    
    [oPanel setAllowsMultipleSelection:selectMultiple];
    [oPanel setCanChooseDirectories: NO];
    [oPanel setCanChooseFiles: YES];
    
    NSWindow* parentWindow = nil;
    if(parent != 0)
    {
        NSView* view = (NSView*)parent->getHandle();
        parentWindow = [view window];
    }
    
    NSMutableArray* allowedFileTypes = [[NSMutableArray alloc] init];
    
    //Allowed file types
    if(wildcard != 0)
    {
        if(convertWildcards(wildcard, allowedFileTypes))
        {
            [oPanel setAllowedFileTypes:allowedFileTypes];
        }
    }
    
    [oPanel beginSheetModalForWindow:parentWindow completionHandler:^(NSInteger returnCode)
     {
         NativeDialogResult result = CANCEL;
         if(returnCode == NSOKButton)
         {
             for(NSURL *url in [oPanel URLs])
             {
                 NSURL *file = [url filePathURL];
                 NSString* absoluteFile = [file path];
                 setPathString([absoluteFile cStringUsingEncoding:NSASCIIStringEncoding]);
             }
             result = OK;
         }
         resultCallback(result);
     }];
    
    [allowedFileTypes release];
    
    [pool release];
}

extern "C" _AnomalousExport void FileSaveDialog_showModal(NativeOSWindow* parent, String message, String defaultDir, String defaultFile, String wildcard, FileSaveDialogResultCallback resultCallback)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSSavePanel *oPanel = [NSSavePanel savePanel];
    
    NSWindow* parentWindow = nil;
    if(parent != 0)
    {
        NSView* view = (NSView*)parent->getHandle();
        parentWindow = [view window];
    }
    
    NSMutableArray* allowedFileTypes = [[NSMutableArray alloc] init];
    
    //Allowed file types
    if(wildcard != 0)
    {
        if(convertWildcards(wildcard, allowedFileTypes))
        {
            [oPanel setAllowedFileTypes:allowedFileTypes];
        }
    }
    
    [oPanel beginSheetModalForWindow:parentWindow completionHandler:^(NSInteger returnCode)
     {
         std::string resPath;
         NativeDialogResult result = CANCEL;
         if(returnCode == NSOKButton)
         {
             NSURL *file = [oPanel URL];
             NSString* absoluteFile = [file path];
             resPath = [absoluteFile cStringUsingEncoding:NSASCIIStringEncoding];
             result = OK;
         }
         resultCallback(result, resPath.c_str());
     }];
    
    [allowedFileTypes release];
    
    [pool release];
}

extern "C" _AnomalousExport void DirDialog_showModal(NativeOSWindow* parent, String message, String startPath, DirDialogResultCallback resultCallback)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSOpenPanel *oPanel = [NSOpenPanel openPanel];
    
    [oPanel setAllowsMultipleSelection:NO];
    [oPanel setCanChooseDirectories: YES];
    [oPanel setCanChooseFiles: NO];
    
    NSWindow* parentWindow = nil;
    if(parent != 0)
    {
        NSView* view = (NSView*)parent->getHandle();
        parentWindow = [view window];
    }
    
    [oPanel beginSheetModalForWindow:parentWindow completionHandler:^(NSInteger returnCode)
     {
         std::string resPath;
         NativeDialogResult result = CANCEL;
         if(returnCode == NSOKButton)
         {
             NSURL *file = [oPanel URL];
             NSString* absoluteFile = [file path];
             resPath = [absoluteFile cStringUsingEncoding:NSASCIIStringEncoding];
             result = OK;
         }
         resultCallback(result, resPath.c_str());
     }];
    
    [pool release];
}


@interface CallbackNotification : NSObject
{
    @private
    ColorDialogResultCallback resultCallback;
}

-(id) initWithCallback: (ColorDialogResultCallback)cb;

-(void) colorPickerClosed:(NSNotification *) notification;

@end

@implementation CallbackNotification

-(id) initWithCallback: (ColorDialogResultCallback)cb
{
    self = [super init];
    resultCallback = cb;
    return self;
}

-(void) colorPickerClosed:(NSNotification *) notification
{
    NSColorPanel* cPanel = [NSColorPanel sharedColorPanel];
    NSColor* theColor = [[cPanel color] colorUsingColorSpaceName:NSCalibratedRGBColorSpace];
    
    NativeDialogResult result = OK;
    Color resColor([theColor redComponent], [theColor greenComponent], [theColor blueComponent]);
    //Color resColor(255.0f, 0.0f, 0.0f);
 
    [[NSNotificationCenter defaultCenter] removeObserver:self name: NSWindowWillCloseNotification object: cPanel];
    
    resultCallback(result, resColor);
    
    [self release];
}

@end

//The current method in this file appears to be working, however it has 2 problems, the first is that the callbackNotificatoin instances are leaking cause you are not releasing them (commented out) and the second time you call the panel it crashes on close. Also the color that comes back through is wrong, but it will not change until the window is closed, which is progress
 
extern "C" _AnomalousExport void ColorDialog_showModal(NativeOSWindow* parent, Color color, ColorDialogResultCallback resultCallback)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSColorPanel* cPanel = [NSColorPanel sharedColorPanel];
    CallbackNotification* cbNotification = [[CallbackNotification alloc] initWithCallback:resultCallback];
    
    [[NSNotificationCenter defaultCenter] addObserver: cbNotification selector: @selector( colorPickerClosed: ) name: NSWindowWillCloseNotification object: cPanel];
    
    NSWindow* parentWindow = nil;
    if(parent != 0)
    {
        NSView* view = (NSView*)parent->getHandle();
        parentWindow = [view window];
    }
    
    [cPanel makeKeyAndOrderFront: cPanel];
    
    [pool release];
}