import { css, html, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';

/**
 * Read-only viewer for a QR code. The value is either the raw code (a data URI, raw base64 PNG,
 * or SVG markup) or the `{ value, qrCode }` payload produced by Generate QR Code's
 * `QrCodeViewerValue` output, which additionally displays the encoded value as text below the
 * code — Umbraco's property editor pipeline auto-deserializes the stored JSON before it reaches
 * this element, so it arrives here as a plain object already, not a string. Offers a Remove
 * button to clear the value, but no way to set or type one — that's done by an Automate workflow
 * (Generate QR Code -> Update Content Property).
 */
export class SaQrCodeViewerElement extends UmbLitElement {
	static properties = {
		value: { type: String },
	};

	async #onRemove() {
		try {
			await umbConfirmModal(this, {
				headline: 'Remove QR code',
				content: 'Are you sure you want to remove this QR code?',
				color: 'danger',
				confirmLabel: 'Remove',
			});
		} catch {
			return; // user cancelled
		}

		this.value = undefined;
		this.dispatchEvent(new UmbChangeEvent());
	}

	// Umbraco's backoffice property editor pipeline auto-deserializes any property value that looks
	// like JSON before it reaches this element (see DataValueEditor.ToEditor), so the
	// QrCodeViewerValue payload arrives here already parsed into a plain object — not a string to
	// JSON.parse ourselves. A legacy raw code (SVG markup, base64, or a data URI) isn't valid JSON,
	// so it's untouched and arrives as a plain string.
	#parseValue() {
		const value = this.value;

		if (value && typeof value === 'object' && typeof value.qrCode === 'string') {
			return { code: value.qrCode, label: typeof value.value === 'string' ? value.value : undefined };
		}

		return { code: value, label: undefined };
	}

	#renderImage(code) {
		if (typeof code !== 'string') return null;

		const trimmed = code.trim();

		if (trimmed.startsWith('<svg')) {
			return html`<div class="qr-svg">${unsafeHTML(trimmed)}</div>`;
		}

		const src = trimmed.startsWith('data:') ? trimmed : `data:image/png;base64,${trimmed}`;
		return html`<img src=${src} alt="QR code" />`;
	}

	render() {
		if (!this.value) {
			return html`<div class="empty">No QR code generated.</div>`;
		}

		const { code, label } = this.#parseValue();
		const image = this.#renderImage(code);

		if (!image) {
			return html`<div class="empty">No QR code generated.</div>`;
		}

		return html`
			<div class="qr-image">
				${image}
				<uui-action-bar class="actions">
					<uui-button label="Remove" look="secondary" @click=${() => this.#onRemove()}>
						<uui-icon name="icon-trash"></uui-icon>
					</uui-button>
				</uui-action-bar>
			</div>
			${label ? html`<div class="qr-value" title=${label}>${label}</div>` : ''}
		`;
	}

	static styles = css`
		.qr-image {
			position: relative;
			display: inline-block;
		}

		.qr-image .actions {
			position: absolute;
			top: var(--uui-size-space-2);
			right: var(--uui-size-space-2);
			opacity: 0;
			transition: opacity 120ms ease-in-out;
		}

		.qr-image:hover .actions,
		.qr-image:focus-within .actions {
			opacity: 1;
		}

		img,
		.qr-svg svg {
			display: block;
			max-width: 200px;
			max-height: 200px;
		}

		.empty {
			color: var(--uui-color-text-alt);
			font-style: italic;
		}

		.qr-value {
			max-width: 100%;
			margin-top: var(--uui-size-space-2);
			font-family: monospace;
			font-size: 12px;
			color: var(--uui-color-text-alt);
			word-break: break-all;
			background: #f8f8f8;
			padding: var(--uui-size-space-1) var(--uui-size-space-2);
			border-radius: var(--uui-border-radius);
		}
	`;
}

customElements.define('sa-qr-code-viewer', SaQrCodeViewerElement);

export default SaQrCodeViewerElement;
