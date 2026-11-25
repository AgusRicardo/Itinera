import { Directive, ElementRef, OnDestroy, inject, input } from '@angular/core';

@Directive({
  selector: '[appTooltip]',
  host: {
    '(mouseenter)': 'mostrar()',
    '(focus)': 'mostrar()',
    '(mouseleave)': 'ocultar()',
    '(blur)': 'ocultar()',
    '(click)': 'ocultar()',
    '[attr.aria-label]': 'texto() || null'
  }
})
export class TooltipDirective implements OnDestroy {
  private readonly elemento = inject<ElementRef<HTMLElement>>(ElementRef);
  private globo?: HTMLElement;

  readonly texto = input('', { alias: 'appTooltip' });

  ngOnDestroy(): void {
    this.ocultar();
  }

  protected mostrar(): void {
    this.ocultar();

    if (!this.texto()) {
      return;
    }

    const campo = this.elemento.nativeElement.getBoundingClientRect();
    this.globo = document.createElement('div');
    this.globo.className = 'pointer-events-none fixed z-[80] -translate-y-1/2 rounded-lg bg-itinera-foreground px-3 py-1.5 text-xs text-itinera-background shadow-lg';
    this.globo.textContent = this.texto();
    this.globo.style.top = `${campo.top + campo.height / 2}px`;
    this.globo.style.left = `${campo.right + 8}px`;
    document.body.append(this.globo);
  }

  protected ocultar(): void {
    this.globo?.remove();
    this.globo = undefined;
  }
}
