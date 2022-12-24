// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

class ServerConsole {
    constructor(options) {
        options = $.extend({ element: $() }, options);
        
        this.element = options.element;
        this.id = options.id;
    }
    
    addEntry(data) {
        let element = $(this.element);
        
        let messagesContainer = element.find('.h-server-messages');
        
        // For now, the data is the entire message text
        let message = data;
        
        let messageSpan = $('<span class="h-console-message">'+message+'</span>')
        messagesContainer.append(messageSpan)

        let scrollableElement = messagesContainer.get(0);
        scrollableElement.scrollTop = scrollableElement.scrollHeight;
    }
    
    
}

//SteamDedicatedServer.ServerConsole