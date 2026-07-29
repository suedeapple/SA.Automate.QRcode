import { css, html, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';

/**
 * Read-only viewer for a QR code stored as a data URI, raw base64 PNG, or SVG markup. Offers a
 * Remove button to clear the value, but no way to set or type one — that's done by an Automate
 * workflow (Generate QR Code -> Update Content Property).
 */
export class SaQrCodeViewerElement extends UmbLitElement {
	static properties = {
		value: { type: String },
	};

	#onRemove() {
		if (!confirm('Remove this QR code? This cannot be undone until you save.')) return;
		this.value = undefined;
		this.dispatchEvent(new UmbChangeEvent());
	}

	#renderImage() {
		const trimmed = this.value.trim();

		if (trimmed.startsWith('<svg')) {
			return html`<div class="qr-svg">${unsafeHTML(trimmed)}</div>`;
		}

		const src = trimmed.startsWith('data:') ? trimmed : `data:image/png;base64,${trimmed}`;
		return html`<img src=${src} alt="QR code" />`;
	}

	render() {
		if (!this.value) {
			return html`<p class="empty">No QR code generated yet.</p>`;
		}

		return html`
			${this.#renderImage()}
			<uui-button label="Remove" look="secondary" color="danger" @click=${() => this.#onRemove()}></uui-button>
		`;
	}

	static styles = css`
		img,
		.qr-svg svg {
			display: block;
			max-width: 200px;
			max-height: 200px;
			margin-bottom: var(--uui-size-space-3);
		}

		.empty {
			color: var(--uui-color-text-alt);
			font-style: italic;
		}
	`;
}

customElements.define('sa-qr-code-viewer', SaQrCodeViewerElement);

export default SaQrCodeViewerElement;
